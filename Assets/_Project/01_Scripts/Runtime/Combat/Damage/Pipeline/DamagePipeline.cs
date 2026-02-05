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

            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                int before = state.GetFinalDamage();

                step.Apply(ctx, state);

                int after = state.GetFinalDamage();

                // ✅ step.yaku는 내부에서 yaku.* 로그를 개별로 남기니까
                // 파이프라인 단계 요약 로그는 남기지 않는다.
                if (before != after && step.Id != "step.yaku")
                {
                    state.AddLog(
                        step.Id,
                        before,
                        after,
                        "changed"
                    );
                }
            }

            return new AttackResult(
                state.GetFinalDamage(),
                state.SideEffects,
                state.Logs
            );
        }
    }
}
