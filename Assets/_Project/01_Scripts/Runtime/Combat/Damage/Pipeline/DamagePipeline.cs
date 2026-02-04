using System.Collections.Generic;

namespace FourMelds.Combat
{
    public sealed class DamagePipeline
    {
        private readonly List<IDamageStep> _steps;

        public DamagePipeline(IEnumerable<IDamageStep> steps)
        {
            _steps = new List<IDamageStep>(steps);
        }

        public AttackResult Execute(in AttackContext ctx)
        {
            var state = new AttackMutableState();
            var logs = new List<DamageLogEntry>();

            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                int before = state.GetFinalDamage();

                step.Apply(ctx, state);

                int after = state.GetFinalDamage();

                if (before != after)
                {
                    logs.Add(new DamageLogEntry(
                        step.Id,
                        before,
                        after,
                        "changed"
                    ));
                }
            }

            return new AttackResult(
                state.GetFinalDamage(),
                state.SideEffects,
                logs
            );
        }
    }
}