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

            var melds = turnState.Melds;
            if (melds == null)
            {
                Debug.LogError("[CTX] turnState.Melds is null");
                melds = Array.Empty<Project.Core.Melds.MeldState>();
            }

            var meldSnapshots = new List<MeldSnapshot>(melds.Count);

            for (int i = 0; i < melds.Count; i++)
            {
                var m = melds[i];
                if (m == null)
                {
                    Debug.LogWarning("[CTX] MeldState is null");
                    continue;
                }

                var tiles = m.Tiles ?? Array.Empty<int>();
                int firstTile = (tiles.Length > 0) ? tiles[0] : 0;

                if (tiles.Length == 0)
                    Debug.LogWarning($"[CTX] Meld tiles null/empty. meldId={m.MeldId} type={m.Type}");

                var suit = GuessSuitFromTileId(firstTile);

                meldSnapshots.Add(new MeldSnapshot(
                    Type: m.Type,
                    Suit: suit,
                    IsFixed: m.IsFixed,
                    Tiles: tiles
                ));
            }

            int meldCount = meldSnapshots.Count;
            int baseDamage = meldCount * 5;

            // ✅ 머리 타일: TurnState에서 가져온다.
            int headTileId = turnState.HeadTileId;

            return new AttackContext(
                turnIndex: new TurnIndex(GetTurnIndexSafe(turnState)),
                player: PlayerStateSnapshot.FromHp(combatState.PlayerHP),
                enemy: EnemyStateSnapshot.FromHp(combatState.EnemyHP),
                headTileId: headTileId,
                hasHead: headTileId != 0,     // 0이면 머리 없음 처리
                meldCount: meldCount,
                melds: meldSnapshots,
                baseDamage: baseDamage,
                suits: default,
                yakuEffects: Array.Empty<IYakuEffect>(),
                relicEffects: Array.Empty<IRelicEffect>()
            );
        }

        private static int GetTurnIndexSafe(TurnState turnState)
        {
            try
            {
                var prop = turnState.GetType().GetProperty("TurnIndex");
                if (prop != null && prop.PropertyType == typeof(int))
                    return (int)prop.GetValue(turnState);
            }
            catch { }

            return 1;
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
