using System;

namespace FourMelds.Combat
{
    public sealed class YakuStep : IDamageStep
    {
        public string Id => "step.yaku";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            var list = ctx.YakuEffects;
            if (list == null || list.Count == 0)
                return;

            for (int i = 0; i < list.Count; i++)
            {
                var y = list[i];
                if (y == null) continue;

                int before = state.GetFinalDamage();

                try
                {
                    if (y.IsActive(in ctx))
                        y.Apply(in ctx, state);
                }
                catch (Exception)
                {
                    // 최소 방어
                }

                int after = state.GetFinalDamage();
                if (before != after)
                    state.AddLog($"yaku.{y.Id}", before, after, "changed");
            }
        }
    }
}
