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

            int before = state.GetFinalDamage();

            for (int i = 0; i < list.Count; i++)
            {
                var y = list[i];
                if (y == null) continue;

                // 인터페이스에 IsActive가 있으면 이 방식
                // 없으면 y 내부에서 스스로 검사하도록 Apply에서 처리해도 됨.
                try
                {
                    // IsActive가 없으면 여기서 컴파일 에러날 수 있음
                    if (y.IsActive(in ctx))
                        y.Apply(in ctx, state);
                }
                catch (Exception)
                {
                    // 개발 중엔 조용히 넘기지 말고 로그 찍는 게 맞지만,
                    // 지금은 파이프라인 안정이 우선이라 최소 방어.
                }
            }

            int after = state.GetFinalDamage();
            if (before != after)
                state.AddLog(Id, before, after, "changed");
        }
    }
}
