namespace FourMelds.Combat
{
    public sealed record DamageLogEntry(
        string StepId,
        int BeforeDamage,
        int AfterDamage,
        string Reason
    );
}