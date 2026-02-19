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

        // ✅ 머리 타일 (역 판정에 반드시 포함)
        public int HeadTileId { get; }

        public bool HasHead { get; }
        public int MeldCount { get; }
        public IReadOnlyList<MeldSnapshot> Melds { get; }

        public int BaseDamage { get; }
        public SuitSummary Suits { get; }
        public int RinshanTileId { get; }
        public bool HasRinshanTile { get; }
        public bool UsesRinshanTileInMelds { get; }

        public IReadOnlyList<IYakuEffect> YakuEffects { get; }
        public IReadOnlyList<IRelicEffect> RelicEffects { get; }

        public AttackContext(
            TurnIndex turnIndex,
            PlayerStateSnapshot player,
            EnemyStateSnapshot enemy,
            int headTileId,
            bool hasHead,
            int meldCount,
            IReadOnlyList<MeldSnapshot> melds,
            int baseDamage,
            SuitSummary suits,
            int rinshanTileId,
            bool hasRinshanTile,
            bool usesRinshanTileInMelds,
            IReadOnlyList<IYakuEffect> yakuEffects,
            IReadOnlyList<IRelicEffect> relicEffects)
        {
            TurnIndex = turnIndex;
            Player = player;
            Enemy = enemy;

            HeadTileId = headTileId;
            HasHead = hasHead;

            MeldCount = meldCount;
            Melds = melds;

            BaseDamage = baseDamage;
            Suits = suits;
            RinshanTileId = rinshanTileId;
            HasRinshanTile = hasRinshanTile;
            UsesRinshanTileInMelds = usesRinshanTileInMelds;

            YakuEffects = yakuEffects;
            RelicEffects = relicEffects;
        }
    }
}
