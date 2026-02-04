namespace FourMelds.Combat
{
    public sealed record DamageModifier(
        string SourceId,
        ModifyMode Mode,
        int AddValue,
        float MulValue
    );
}