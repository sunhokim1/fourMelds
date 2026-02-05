using System.Collections.Generic;

namespace FourMelds.Combat
{
    public sealed class AttackMutableState
    {
        // 몸통별 데미지 컴포넌트
        public readonly List<MeldDamageComponent> MeldComponents = new();

        // 전체(글로벌) 보정
        public int GlobalBonus = 0;
        public float GlobalMult = 1f;

        // Clamp/Compression 같이 "최종값을 강제로 결정"해야 할 때 사용
        public int? FinalDamageOverride = null;

        public readonly List<DamageModifier> Modifiers = new();
        public readonly List<AttackSideEffect> SideEffects = new();
        public readonly List<DamageLogEntry> Logs = new();
        public void AddLog(string stepId, int before, int after, string reason)
            => Logs.Add(new DamageLogEntry(stepId, before, after, reason));


        internal int EvaluateFinalDamage()
        {
            int sum = 0;
            for (int i = 0; i < MeldComponents.Count; i++)
                sum += MeldComponents[i].Evaluate();

            float v = (sum + GlobalBonus) * GlobalMult;
            return (int)v;
        }


        public int GetFinalDamage()
        {
            return FinalDamageOverride.HasValue ? FinalDamageOverride.Value : EvaluateFinalDamage();
        }
    }
}