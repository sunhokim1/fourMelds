namespace FourMelds.Combat
{
    public sealed record AttackSideEffect(
        SideEffectType Type,
        int Magnitude,
        string Tag,
        string SourceId
    );
}