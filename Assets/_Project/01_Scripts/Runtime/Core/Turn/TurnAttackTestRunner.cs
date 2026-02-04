using System.Collections.Generic;
using UnityEngine;

using Project.Core.Melds; // MeldType

namespace FourMelds.Combat
{
    public sealed class TurnAttackTestRunner : MonoBehaviour
    {
        void Start()
        {
            // 예시: 만수 1 / 통수(=Pinzu) 2 / 삭수 1
            var melds = new List<MeldSnapshot>
            {
                new MeldSnapshot(MeldType.Shuntsu, SuitType.Manzu, false),
                new MeldSnapshot(MeldType.Koutsu,  SuitType.Pinzu, false),
                new MeldSnapshot(MeldType.Shuntsu, SuitType.Pinzu, false),
                new MeldSnapshot(MeldType.Kantsu,  SuitType.Souzu, true),
            };

            // SuitSummary는 named argument 대신 "순서대로" 넣는다.
            // (Manzu, Souzu, Pinzu, Honors, IsTanyaoLike, IsMonoSuit, IsMixedSuit)
            var suits = new SuitSummary(
                1,      // manzuCount
                1,      // souzuCount
                2,      // pinzuCount
                0,      // honorsCount
                false,  // isTanyaoLike
                false,  // isMonoSuit
                true    // isMixedSuit
            );

            var relics = new List<IRelicEffect>
            {
                new Relic_ManzuMeldBonus()
            };

            var yakus = new List<IYakuEffect>();

            var ctx = new AttackContext(
                new TurnIndex(1),
                new PlayerStateSnapshot(50, 50, 0, 0, 0),
                new EnemyStateSnapshot(30, 30, 0, 1),
                true,
                melds.Count,
                melds,
                5,
                suits,
                yakus,
                relics
            );

            var pipeline = new DamagePipeline(new IDamageStep[]
            {
                new BaseDamageStep(),
                new SuitDamageStep(),
                new YakuStep(),
                new RelicStep(),
                new ClampStep()
            });

            AttackResult result = pipeline.Execute(ctx);

            Debug.Log("========== ATTACK TEST ==========");
            Debug.Log($"FinalDamage = {result.FinalDamage}");

            for (int i = 0; i < result.LogEntries.Count; i++)
            {
                var log = result.LogEntries[i];
                Debug.Log($"[{log.StepId}] {log.BeforeDamage} -> {log.AfterDamage} ({log.Reason})");
            }

            Debug.Log("=================================");
        }
    }
}
