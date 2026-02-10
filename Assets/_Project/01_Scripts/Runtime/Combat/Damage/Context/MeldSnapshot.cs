using System.Collections.Generic;
using Project.Core.Melds;
using FourMelds.Core.Suits;

namespace FourMelds.Combat
{
    public sealed record MeldSnapshot(
        MeldType Type,
        MahjongSuit Suit,
        bool IsFixed,
        IReadOnlyList<int> Tiles
    );
}
