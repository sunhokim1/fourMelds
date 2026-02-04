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

                int firstTile = 0;

                // tiles 배열이 null일 수 있으니 완전 방어
                var tiles = m.Tiles;
                if (tiles != null && tiles.Length > 0)
                    firstTile = tiles[0];
                else
                    Debug.LogWarning($"[CTX] Meld tiles null/empty. meldId={m.MeldId} type={m.Type}");

                var suit = GuessSuitFromTileId(firstTile);

                meldSnapshots.Add(new MeldSnapshot(
                    Type: m.Type,
                    Suit: suit,
                    IsFixed: m.IsFixed
                ));
            }

            int meldCount = meldSnapshots.Count;
            int baseDamage = meldCount * 5;

            return new AttackContext(
                turnIndex: new TurnIndex(GetTurnIndexSafe(turnState)),
                player: PlayerStateSnapshot.FromHp(combatState.PlayerHP),
                enemy: EnemyStateSnapshot.FromHp(combatState.EnemyHP),
                hasHead: true,
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
            // TurnState에 TurnIndex가 없을 수도 있으니,
            // 일단 1로 고정 (Day3).
            // 나중에 TurnState에 TurnIndex 넣으면 여기 제거.
            try
            {
                // 만약 TurnState에 TurnIndex가 있으면 사용
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
