using FourMelds.Core.Suits;

namespace FourMelds.Combat
{
    public sealed class Relic_ManzuMeldBonus : IRelicEffect
    {
        public string Id => "relic.manzu_meld_bonus";

        public bool IsActive(in AttackContext ctx) => true;

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            for (int i = 0; i < state.MeldComponents.Count; i++)
            {
                var comp = state.MeldComponents[i];
                if (comp.Suit == MahjongSuit.Manzu)
                    comp.Bonus += 3; // ✅ 만수 몸통에 해당하는 데미지에만 적용
            }
        }
    }
}