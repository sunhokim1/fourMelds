using Project.Core.Melds;

namespace FourMelds.Combat
{
    public sealed record MeldSnapshot(
        MeldType Type,
        SuitType Suit,
        bool IsFixed
    );
}