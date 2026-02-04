namespace FourMelds.Combat
{
    public sealed record PlayerStateSnapshot(
        int Hp,
        int MaxHp,
        int Armor,
        int Energy,
        int ComboCounter
    );
}