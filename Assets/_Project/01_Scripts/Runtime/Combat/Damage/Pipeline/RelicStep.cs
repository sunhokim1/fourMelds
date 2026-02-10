namespace FourMelds.Combat
{
    public sealed class RelicStep : IDamageStep
    {
        public string Id => "step.relic";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            if (ctx.RelicEffects == null || ctx.RelicEffects.Count == 0)
                return;

            for (int i = 0; i < ctx.RelicEffects.Count; i++)
            {
                var relic = ctx.RelicEffects[i];
                if (relic == null) continue;

                if (!relic.IsActive(ctx)) continue;

                relic.Apply(ctx, state);
            }
        }
    }
}