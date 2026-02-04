using System.Collections.Generic;

namespace FourMelds.Combat
{
    public sealed class YakuStep : IDamageStep
    {
        public string Id => "step.yaku";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            if (ctx.YakuEffects == null || ctx.YakuEffects.Count == 0)
                return;

            int appliedCount = 0;

            for (int i = 0; i < ctx.YakuEffects.Count; i++)
            {
                var yaku = ctx.YakuEffects[i];
                if (yaku == null) continue;

                if (!yaku.IsActive(ctx)) continue;

                yaku.Apply(ctx, state);
                appliedCount++;
            }

            // 로그는 DamagePipeline에서 before/after로 찍히고,
            // 여기선 수치변경이 없더라도 "뭔가 적용됨"을 남기고 싶으면
            // state에 별도 디버그 메모 리스트를 추가하는 방식이 Day 3~4에서 좋음.
        }
    }
}