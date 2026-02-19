using System;
using System.Collections.Generic;
using Project.Core.Turn;
using FourMelds.Core.Suits;
using UnityEngine;

namespace FourMelds.Combat.TurnIntegration
{
    public sealed class TurnAttackContextBuilder
    {
        public AttackContext Build(TurnState turnState, CombatState combatState)
        {
            if (turnState == null)
            {
                Debug.LogError("[CTX] turnState is null");
                throw new ArgumentNullException(nameof(turnState));
            }
            if (combatState == null)
            {
                Debug.LogError("[CTX] combatState is null");
                throw new ArgumentNullException(nameof(combatState));
            }

            var meldSnapshots = BuildMeldSnapshots(turnState);

            // SuitSummary 계산용
            int manzu = 0, souzu = 0, pinzu = 0, honors = 0;

            // TanyaoLike(2~8 숫자패만) 체크용
            bool isTanyaoLike = true;

            for (int i = 0; i < meldSnapshots.Count; i++)
            {
                var m = meldSnapshots[i];
                var tiles = m.Tiles ?? Array.Empty<int>();
                int firstTile = (tiles.Length > 0) ? tiles[0] : 0;

                if (tiles.Length == 0)
                    Debug.LogWarning($"[CTX] Meld tiles null/empty. type={m.Type}");

                var suit = GuessSuitFromTileId(firstTile);
                AddSuitCount(suit, ref manzu, ref souzu, ref pinzu, ref honors);

                // TanyaoLike: 멜드에 포함된 타일이 하나라도 1/9/자패면 false
                for (int t = 0; t < tiles.Length; t++)
                {
                    int id = tiles[t];
                    int group = id / 100;
                    int rank = id % 100;

                    if (group == 4) { isTanyaoLike = false; break; }      // honor
                    if (rank < 2 || rank > 8) { isTanyaoLike = false; break; } // 1/9
                }

            }

            int meldCount = meldSnapshots.Count;
            int baseDamage = meldCount * 5;
            int rinshanTileId = turnState.RinshanTileId;
            bool hasRinshanTile = rinshanTileId != 0;
            bool usesRinshanTileInMelds = hasRinshanTile && IsTileUsedInMelds(rinshanTileId, meldSnapshots);

            // ✅ Head
            int headTileId = turnState.HeadTileId;
            bool hasHead = headTileId != 0;

            if (hasHead)
            {
                var headSuit = GuessSuitFromTileId(headTileId);
                AddSuitCount(headSuit, ref manzu, ref souzu, ref pinzu, ref honors);

                // TanyaoLike에 Head도 포함
                int group = headTileId / 100;
                int rank = headTileId % 100;
                if (group == 4) isTanyaoLike = false;
                if (rank < 2 || rank > 8) isTanyaoLike = false;
            }
            else
            {
                // 네 설계대로 "Head 없으면 탕야오 같은 건 성립 안 함" 쪽으로 맞추기
                isTanyaoLike = false;
            }

            // Mono/Mixed 계산
            int suitKinds =
                (manzu > 0 ? 1 : 0) +
                (souzu > 0 ? 1 : 0) +
                (pinzu > 0 ? 1 : 0);

            bool isMonoSuit = (suitKinds == 1) && (honors == 0);
            bool isMixedSuit = (suitKinds >= 2); // honors는 mixed 판정에 굳이 포함 안 함 (필요하면 바꿔도 됨)

            var suits = new SuitSummary(
                ManzuCount: manzu,
                SouzuCount: souzu,
                PinzuCount: pinzu,
                HonorsCount: honors,
                IsTanyaoLike: isTanyaoLike,
                IsMonoSuit: isMonoSuit,
                IsMixedSuit: isMixedSuit
            );

            return new AttackContext(
                turnIndex: new TurnIndex(turnState.TurnIndex),
                player: PlayerStateSnapshot.FromHp(combatState.PlayerHP),
                enemy: EnemyStateSnapshot.FromHp(combatState.EnemyHP),
                headTileId: headTileId,
                hasHead: hasHead,
                meldCount: meldCount,
                melds: meldSnapshots,
                baseDamage: baseDamage,
                suits: suits,
                rinshanTileId: rinshanTileId,
                hasRinshanTile: hasRinshanTile,
                usesRinshanTileInMelds: usesRinshanTileInMelds,
                yakuEffects: YakuRegistry.CreateDefault(),
                relicEffects: Array.Empty<IRelicEffect>()
            );
        }

        private static List<MeldSnapshot> BuildMeldSnapshots(TurnState turnState)
        {
            var list = new List<MeldSnapshot>(4);
            for (int slot = 0; slot < 4; slot++)
            {
                var slotTilesReadonly = turnState.GetSlotTiles(slot);
                if (slotTilesReadonly == null || slotTilesReadonly.Count < 3)
                    continue;

                var slotTiles = new int[slotTilesReadonly.Count];
                for (int i = 0; i < slotTiles.Length; i++)
                    slotTiles[i] = slotTilesReadonly[i];

                if (!TryResolveMeldType(slotTiles, out var type))
                    continue;

                int firstTile = slotTiles[0];
                list.Add(new MeldSnapshot(
                    Type: type,
                    Suit: GuessSuitFromTileId(firstTile),
                    IsFixed: turnState.IsSlotFixed(slot),
                    Tiles: slotTiles));
            }

            if (list.Count > 0)
                return list;

            // Fallback for legacy/dev paths that still populate TurnState.Melds.
            var melds = turnState.Melds ?? Array.Empty<Project.Core.Melds.MeldState>();
            for (int i = 0; i < melds.Count; i++)
            {
                var m = melds[i];
                if (m == null)
                    continue;
                var tiles = m.Tiles ?? Array.Empty<int>();
                int firstTile = tiles.Length > 0 ? tiles[0] : 0;
                list.Add(new MeldSnapshot(
                    Type: m.Type,
                    Suit: GuessSuitFromTileId(firstTile),
                    IsFixed: m.IsFixed,
                    Tiles: tiles));
            }

            return list;
        }

        private static bool TryResolveMeldType(int[] tiles, out Project.Core.Melds.MeldType type)
        {
            type = Project.Core.Melds.MeldType.Shuntsu;
            if (tiles == null || tiles.Length < 3)
                return false;

            bool allSame = true;
            for (int i = 1; i < tiles.Length; i++)
                if (tiles[i] != tiles[0]) { allSame = false; break; }

            if (allSame && tiles.Length == 4)
            {
                type = Project.Core.Melds.MeldType.Kantsu;
                return true;
            }

            if (allSame && tiles.Length == 3)
            {
                type = Project.Core.Melds.MeldType.Koutsu;
                return true;
            }

            if (tiles.Length != 3)
                return false;

            Array.Sort(tiles);
            int a = tiles[0], b = tiles[1], c = tiles[2];
            int suit = a / 100;
            if (suit < 1 || suit > 3 || b / 100 != suit || c / 100 != suit)
                return false;

            int ra = a % 100, rb = b % 100, rc = c % 100;
            if (rb == ra + 1 && rc == rb + 1)
            {
                type = Project.Core.Melds.MeldType.Shuntsu;
                return true;
            }

            return false;
        }

        private static bool IsTileUsedInMelds(int tileId, IReadOnlyList<MeldSnapshot> melds)
        {
            if (tileId == 0 || melds == null)
                return false;

            for (int i = 0; i < melds.Count; i++)
            {
                var tiles = melds[i].Tiles;
                if (tiles == null)
                    continue;
                for (int t = 0; t < tiles.Count; t++)
                    if (tiles[t] == tileId)
                        return true;
            }

            return false;
        }

        private static void AddSuitCount(MahjongSuit suit, ref int manzu, ref int souzu, ref int pinzu, ref int honors)
        {
            switch (suit)
            {
                case MahjongSuit.Manzu: manzu++; break;
                case MahjongSuit.Souzu: souzu++; break;
                case MahjongSuit.Pinzu: pinzu++; break;
                default: honors++; break;
            }
        }

        private static MahjongSuit GuessSuitFromTileId(int tileId)
        {
            int group = tileId / 100;
            return group switch
            {
                1 => MahjongSuit.Manzu,
                2 => MahjongSuit.Souzu,
                3 => MahjongSuit.Pinzu,
                _ => MahjongSuit.Honor
            };
        }
    }
}
