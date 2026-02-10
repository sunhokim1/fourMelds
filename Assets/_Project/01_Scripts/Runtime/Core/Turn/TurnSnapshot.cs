using System.Collections.Generic;
using FourMelds.Core.Turn;

namespace Project.Core.Turn
{
    public class TurnSnapshot
    {
        public TurnPhase Phase { get; }
        public int TurnIndex { get; }
        public IReadOnlyList<int> HandTiles { get; }
        public IReadOnlyList<int> MeldIds { get; }

        public TurnSnapshot(
            TurnPhase phase,
            int turnIndex,
            IReadOnlyList<int> handTiles,
            IReadOnlyList<int> meldIds)
        {
            Phase = phase;
            TurnIndex = turnIndex;
            HandTiles = handTiles;
            MeldIds = meldIds;
        }
    }
}