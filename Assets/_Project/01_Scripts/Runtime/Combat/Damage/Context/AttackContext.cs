using System.Collections.Generic;

namespace FourMelds.Combat
{
    /// <summary>
    /// C# 9.0 호환: record struct 대신 readonly struct + 생성자 사용
    /// </summary>
    public readonly struct AttackContext
    {
        public TurnIndex TurnIndex { get; }
        public PlayerStateSnapshot Player { get; }
        public EnemyStateSnapshot Enemy { get; }

        public bool HasHead { get; }
        public int MeldCount { get; }
        public IReadOnlyList<MeldSnapshot> Melds { get; }

        public int BaseDamage { get; }
        public SuitSummary Suits { get; }

        public IReadOnlyList<IYakuEffect> YakuEffects { get; }
        public IReadOnlyList<IRelicEffect> RelicEffects { get; }

        public AttackContext(
            TurnIndex turnIndex,
            PlayerStateSnapshot player,
            EnemyStateSnapshot enemy,
            bool hasHead,
            int meldCount,
            IReadOnlyList<MeldSnapshot> melds,
            int baseDamage,
            SuitSummary suits,
            IReadOnlyList<IYakuEffect> yakuEffects,
            IReadOnlyList<IRelicEffect> relicEffects)
        {
            TurnIndex = turnIndex;
            Player = player;
            Enemy = enemy;

            HasHead = hasHead;
            MeldCount = meldCount;
            Melds = melds;

            BaseDamage = baseDamage;
            Suits = suits;

            YakuEffects = yakuEffects;
            RelicEffects = relicEffects;
        }
    }
}