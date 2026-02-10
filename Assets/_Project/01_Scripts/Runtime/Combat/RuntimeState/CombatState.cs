// Assets/_Project/01_Scripts/Runtime/Combat/RuntimeState/CombatState.cs

namespace FourMelds.Combat
{
    public sealed class CombatState
    {
        public int PlayerHP { get; private set; }
        public int EnemyHP { get; private set; }

        public CombatState(int playerHp, int enemyHp)
        {
            PlayerHP = playerHp;
            EnemyHP = enemyHp;
        }

        public void ApplyEnemyDamage(int damage)
        {
            if (damage < 0) damage = 0;
            EnemyHP -= damage;
            if (EnemyHP < 0) EnemyHP = 0;
        }

        public void ApplyPlayerDamage(int damage)
        {
            if (damage < 0) damage = 0;
            PlayerHP -= damage;
            if (PlayerHP < 0) PlayerHP = 0;
        }

        public void ApplyPlayerHeal(int amount)
        {
            if (amount <= 0) return;
            PlayerHP += amount;
        }
    }
}
