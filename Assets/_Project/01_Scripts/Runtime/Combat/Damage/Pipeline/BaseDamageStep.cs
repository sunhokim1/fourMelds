namespace FourMelds.Combat
{
    /// <summary>
    /// 몸통별 데미지 컴포넌트를 생성한다.
    /// (여기서 "몸통 1개가 기본적으로 얼마를 주는지" 룰이 결정됨)
    /// </summary>
    public sealed class BaseDamageStep : IDamageStep
    {
        public string Id => "step.base";

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.MeldComponents.Clear();
            state.FinalDamageOverride = null;
            state.GlobalBonus = 0;
            state.GlobalMult = 1f;

            // ✅ 임시 룰: 몸통 1개 base = ctx.BaseDamage
            // (나중에: MeldType에 따라 base 달리기 가능)
            for (int i = 0; i < ctx.Melds.Count; i++)
            {
                var m = ctx.Melds[i];
                state.MeldComponents.Add(new MeldDamageComponent(
                    i,
                    m.Type,
                    m.Suit,
                    ctx.BaseDamage
                ));
            }

            // 머리 자동 제공을 "최소 공격 가능" 조건으로 쓰고 싶으면
            // 여기서 GlobalBonus 같은 걸 줄 수도 있음.
        }
    }
}