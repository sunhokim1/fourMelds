namespace FourMelds.Combat
{
    public sealed record SuitSummary(
        int ManzuCount,
        int SouzuCount,
        int PinzuCount,
        int HonorsCount,
        bool IsTanyaoLike,
        bool IsMonoSuit,
        bool IsMixedSuit
    );
}