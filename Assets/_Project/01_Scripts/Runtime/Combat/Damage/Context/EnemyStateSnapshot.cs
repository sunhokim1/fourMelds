namespace FourMelds.Combat
{
    public sealed record EnemyStateSnapshot(
        int Hp,
        int MaxHp,
        int Armor,
        int Level
    );
}