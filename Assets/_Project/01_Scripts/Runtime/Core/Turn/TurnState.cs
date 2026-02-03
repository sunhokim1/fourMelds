using System.Collections.Generic;
using Project.Core.Melds;

namespace Project.Core.Turn
{
    public class TurnState
    {
        private readonly List<int> _handTiles;
        private readonly List<MeldState> _melds = new();
        private int _nextMeldId = 1;

        public IReadOnlyList<int> HandTiles => _handTiles;
        public IReadOnlyList<MeldState> Melds => _melds;

        public TurnState(IEnumerable<int> initialTiles)
        {
            _handTiles = new List<int>(initialTiles);
        }

        public int CreateMeld(MeldType type, int[] tiles, bool fixedNow = false)
        {
            var meld = new MeldState(_nextMeldId++, type, tiles, fixedNow);
            _melds.Add(meld);
            return meld.MeldId;
        }

        public bool TryRemoveTile(int tileId)
        {
            int idx = _handTiles.IndexOf(tileId);
            if (idx < 0) return false;
            _handTiles.RemoveAt(idx);
            return true;
        }

        public int CountOf(int tileId)
        {
            int c = 0;
            for (int i = 0; i < _handTiles.Count; i++)
                if (_handTiles[i] == tileId) c++;
            return c;
        }
    }
}