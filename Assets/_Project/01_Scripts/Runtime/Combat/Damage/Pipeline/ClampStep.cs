namespace FourMelds.Combat
{
    public sealed class ClampStep : IDamageStep
    {
        public string Id => "step.clamp";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            int v = state.EvaluateFinalDamage();

            if (v < 0) v = 0;
            if (v > 999) v = 999;

            state.FinalDamageOverride = v;
        }
    }
}