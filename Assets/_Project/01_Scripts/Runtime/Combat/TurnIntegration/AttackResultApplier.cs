// Assets/_Project/01_Scripts/Runtime/Combat/TurnIntegration/AttackResultApplier.cs

namespace FourMelds.Combat.TurnIntegration
{
    public sealed class AttackResultApplier
    {
        public void Apply(CombatState combatState, in AttackResult result)
        {
            combatState.ApplyEnemyDamage(result.FinalDamage);
            // Day3: SideEffects´Â ÀÏ´Ü ½ºÅµ
        }
    }
}
