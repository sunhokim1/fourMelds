namespace FourMelds.Combat
{
    public sealed class SuitDamageStep : IDamageStep
    {
        public string Id => "step.suit";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            // ✅ 예시: 단색이면 전체 배율
            if (ctx.Suits.IsMonoSuit)
                state.GlobalMult *= 1.2f;
        }
    }
}