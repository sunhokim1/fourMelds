using System.Collections.Generic;
using FourMelds.Core.Turn;
using Project.Core.Melds;

namespace Project.Core.Turn
{
    public class TurnState
    {
        private readonly List<int> _handTiles;
        private readonly List<MeldState> _melds = new();
        private int _nextMeldId = 1;


        public TurnPhase Phase { get; private set; } = TurnPhase.Build; // Day3: 일단 Build부터 시작
        public int TurnIndex { get; private set; } = 1;

        public IReadOnlyList<int> HandTiles => _handTiles;
        public IReadOnlyList<MeldState> Melds => _melds;

        public TurnState(IEnumerable<int> initialTiles)
        {
            _handTiles = new List<int>(initialTiles);
        }

        public void SetPhase(TurnPhase phase) => Phase = phase;

        public void AdvanceTurn()
        {
            TurnIndex++;
            Phase = TurnPhase.RoundStart;
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

        // --- Day3+ 턴 정리용 (룰: 턴 끝나면 전부 소멸) ---

        public void ClearAllHandTiles() => _handTiles.Clear();

        public void ClearAllMelds(bool resetMeldId = true)
        {
            _melds.Clear();
            if (resetMeldId) _nextMeldId = 1;
        }
    }
}
