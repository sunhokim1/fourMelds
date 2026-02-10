using FourMelds.Core.Suits;
using Project.Core.Melds;

namespace FourMelds.Combat
{
    /// <summary>
    /// 몸통 1개가 기여하는 데미지 조각.
    /// 특정 슈트/특정 몸통에만 보정을 넣기 위해 존재한다.
    /// </summary>
    public sealed class MeldDamageComponent
    {
        public int Index { get; }
        public MeldType MeldType { get; }
        public MahjongSuit Suit { get; }

        public int Base;
        public int Bonus;
        public float Mult;

        public MeldDamageComponent(int index, MeldType meldType, MahjongSuit suit, int baseValue)
        {
            Index = index;
            MeldType = meldType;
            Suit = suit;

            Base = baseValue;
            Bonus = 0;
            Mult = 1f;
        }

        public int Evaluate()
        {
            float v = (Base + Bonus) * Mult;
            return (int)v;
        }
    }
}