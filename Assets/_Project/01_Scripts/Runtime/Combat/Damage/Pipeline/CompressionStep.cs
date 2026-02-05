using System;

namespace FourMelds.Combat
{
    /// <summary>
    /// Day5: knee 이하에서는 데미지를 건드리지 않고,
    /// knee 초과 구간만 로그 곡선으로 압축한다.
    /// - 절대 원본보다 커지지 않는다.
    /// </summary>
    public sealed class CompressionStep : IDamageStep
    {
        public string Id => "step.compression";

        private readonly int _knee;
        private readonly float _strength;

        /// <param name="knee">이 값 이하에서는 압축 없음. 이 값부터 눌리기 시작.</param>
        /// <param name="strength">1.0 = 기본. 1보다 크면 더 강하게 눌림(단, 증가 버그 없음)</param>
        public CompressionStep(int knee = 30, float strength = 1.0f)
        {
            if (knee < 1) knee = 1;
            if (strength <= 0f) strength = 0.01f;

            _knee = knee;
            _strength = strength;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            int x = state.GetFinalDamage();
            if (x <= 0)
            {
                state.FinalDamageOverride = 0;
                return;
            }

            // knee 이하는 그대로 (압축으로 증가/감소 없음)
            if (x <= _knee)
            {
                state.FinalDamageOverride = x;
                return;
            }

            int excess = x - _knee;

            // 초과분만 로그로 압축: knee + knee * ln(1 + strength * excess / knee)
            double k = _knee;
            double s = _strength;
            double e = excess;

            double compressedExcess = k * Math.Log(1.0 + (s * e / k));
            int y = _knee + (int)Math.Round(compressedExcess);

            if (y < 0) y = 0;
            if (y > x) y = x; // 안전장치(수학적으로는 거의 안 걸리지만 확실히)

            state.FinalDamageOverride = y;
        }
    }
}
