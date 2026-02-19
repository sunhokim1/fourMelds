// Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnState.cs

using System;
using System.Collections.Generic;
using FourMelds.Core.Turn;
using Project.Core.Melds;
using Project.Core.Tiles;

namespace Project.Core.Turn
{
    public class TurnState
    {
        private readonly List<int> _handTiles;
        private readonly List<MeldState> _melds = new();
        private int _nextMeldId = 1;

        // Day4: 시작은 Draw로 맞춰두는 게 깔끔 (TurnLoopController.Start()에서도 SetPhase(Draw) 함)
        public TurnPhase Phase { get; private set; } = TurnPhase.Draw;
        public int TurnIndex { get; private set; } = 1;

        public IReadOnlyList<int> HandTiles => _handTiles;
        public IReadOnlyList<MeldState> Melds => _melds;

        // Head는 "손패"가 아니라 별도 슬롯 (A안)
        public int HeadTileId { get; private set; } = 0; // 0이면 아직 없음
        public int RinshanTileId { get; private set; } = 0;
        public int RinshanTileOccurrence { get; private set; } = 0;

        // TilePool은 TurnLoopController(전투 단위)가 만들고 TurnState에 주입한다.
        public TilePool Pool { get; private set; }

        public TurnState(IEnumerable<int> initialTiles)
        {
            _handTiles = new List<int>(initialTiles);
        }

        public void SetPhase(TurnPhase phase) => Phase = phase;

        public void SetPool(TilePool pool)
        {
            Pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }

        public void SetHeadTile(int tileId)
        {
            if (tileId == 0) throw new ArgumentException("Head tileId cannot be 0.", nameof(tileId));
            HeadTileId = tileId;
        }

        public void ClearHead() => HeadTileId = 0;

        public void AddHandTile(int tileId) => _handTiles.Add(tileId);

        public void AdvanceTurn()
        {
            TurnIndex++;
            Phase = TurnPhase.Draw;
        }

        /// <summary>
        /// 룰: 턴 종료 시 손패/몸통 소멸.
        /// Pool 리셋/Head 드로우/초기 손패 드로우는 "턴 시작(Draw)"에서 외부가 처리한다.
        /// </summary>
        public void CleanupForNextTurn()
        {
            // 0) 이번 턴 소멸(규칙)
            ClearAllHandTiles();
            ClearAllMelds(resetMeldId: true);
            ClearAllSlots();

            // 1) Head 초기화 (다음 턴 Draw에서 다시 세팅)
            HeadTileId = 0;
            ClearRinshanState();

            // 2) 턴 진행
            AdvanceTurn(); // TurnIndex++ / Phase=Draw
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
            OnHandTileRemoved(tileId);
            return true;
        }

        public int CountOf(int tileId)
        {
            int c = 0;
            for (int i = 0; i < _handTiles.Count; i++)
                if (_handTiles[i] == tileId) c++;
            return c;
        }

        // --- 턴 정리용 (룰: 턴 끝나면 전부 소멸) ---

        public void ClearAllHandTiles()
        {
            _handTiles.Clear();
            ClearRinshanState();
        }

        public void ClearAllMelds(bool resetMeldId = true)
        {
            _melds.Clear();
            if (resetMeldId) _nextMeldId = 1;
        }

        public void Dev_SetHandTiles(IEnumerable<int> tiles)
        {
            _handTiles.Clear();
            _handTiles.AddRange(tiles);
            ClearRinshanState();
        }

        public void Dev_AddMeld(MeldType type, int[] tiles, bool fixedNow)
        {
            CreateMeld(type, tiles, fixedNow);
        }

        public void Dev_SetMelds(params (MeldType type, bool fixedNow, int[] tiles)[] melds)
        {
            ClearAllMelds(resetMeldId: true);

            for (int i = 0; i < melds.Length; i++)
            {
                var m = melds[i];
                CreateMeld(m.type, m.tiles, m.fixedNow);
            }
        }
        // TurnState.cs 내부에 추가

        public int SelectedSlotIndex { get; private set; } = 0;

        public void SelectSlot(int index)
        {
            if (index < 0) index = 0;
            if (index > 3) index = 3;
            SelectedSlotIndex = index;
        }

        // TurnState.cs 내부에 추가 (using System; using System.Linq; 필요하면 추가)

        private readonly List<int>[] _slotTiles = new List<int>[4]
        {
            new List<int>(4),
            new List<int>(4),
            new List<int>(4),
            new List<int>(4),
        };
        private readonly bool[] _slotFixed = new bool[4];

        public IReadOnlyList<int> GetSlotTiles(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex > 3) return Array.Empty<int>();
            return _slotTiles[slotIndex];
        }

        public bool IsSlotFixed(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex > 3)
                return false;
            return _slotFixed[slotIndex];
        }

        public void ClearAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                _slotTiles[i].Clear();
                _slotFixed[i] = false;
            }
        }

        public bool TryAddTileToSelectedSlot(int tileId, out string reason)
        {
            reason = null;

            int slot = SelectedSlotIndex;
            if (slot < 0 || slot > 3) { reason = "No slot selected"; return false; }
            if (_slotFixed[slot]) { reason = "Slot is fixed"; return false; }

            var slotList = _slotTiles[slot];
            if (slotList.Count >= 3)
            {
                reason = "Slot already has 3 tiles";
                return false;
            }


            // 손패에서 제거
            if (!TryRemoveTile(tileId))
            {
                reason = "Tile not in hand";
                return false;
            }

            // 슬롯에 추가
            slotList.Add(tileId);

            SortHandTiles();
            return true;
        }

        public bool TryRemoveTileFromSlot(int slotIndex, int tileId, out string reason)
        {
            reason = null;

            if (slotIndex < 0 || slotIndex > 3) { reason = "Invalid slot"; return false; }
            if (_slotFixed[slotIndex]) { reason = "Slot is fixed"; return false; }

            var slotList = _slotTiles[slotIndex];
            int idx = slotList.IndexOf(tileId);
            if (idx < 0) { reason = "Tile not in slot"; return false; }

            slotList.RemoveAt(idx);
            AddHandTile(tileId);

            SortHandTiles();
            return true;
        }

        public void SortHandTiles()
        {
            _handTiles.Sort(); // tileId 자체로 정렬(만/삭/통/자 순 + 숫자 순)
        }
        public bool TryClearSlotToHand(int slotIndex, out string reason)
        {
            reason = null;

            if (slotIndex < 0 || slotIndex > 3)
            {
                reason = "Invalid slot";
                return false;
            }
            if (_slotFixed[slotIndex])
            {
                reason = "Slot is fixed";
                return false;
            }

            var slotList = _slotTiles[slotIndex];
            if (slotList.Count == 0)
                return true;

            for (int i = 0; i < slotList.Count; i++)
                AddHandTile(slotList[i]);

            slotList.Clear();
            SortHandTiles();
            return true;
        }

        public bool TryReplaceSlotWithTilesFromHand(int slotIndex, IReadOnlyList<int> tiles, out string reason)
        {
            reason = null;

            if (slotIndex < 0 || slotIndex > 3)
            {
                reason = "Invalid slot";
                return false;
            }
            if (_slotFixed[slotIndex])
            {
                reason = "Slot is fixed";
                return false;
            }

            if (tiles == null || tiles.Count == 0)
            {
                reason = "No tiles to place";
                return false;
            }

            var slotList = _slotTiles[slotIndex];

            // Availability = current hand + tiles currently in the selected slot
            // because slot tiles are returned to hand before replacement.
            var avail = new Dictionary<int, int>();
            for (int i = 0; i < _handTiles.Count; i++)
            {
                int id = _handTiles[i];
                avail[id] = avail.TryGetValue(id, out var c) ? c + 1 : 1;
            }
            for (int i = 0; i < slotList.Count; i++)
            {
                int id = slotList[i];
                avail[id] = avail.TryGetValue(id, out var c) ? c + 1 : 1;
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                int id = tiles[i];
                if (!avail.TryGetValue(id, out var c) || c <= 0)
                {
                    reason = $"Tile missing for replacement: {id}";
                    return false;
                }

                avail[id] = c - 1;
            }

            // 1) Return previous slot tiles to hand.
            for (int i = 0; i < slotList.Count; i++)
                AddHandTile(slotList[i]);
            slotList.Clear();

            // 2) Consume desired tiles from hand and place into selected slot.
            for (int i = 0; i < tiles.Count; i++)
            {
                int id = tiles[i];
                if (!TryRemoveTile(id))
                {
                    reason = $"Failed to consume tile: {id}";
                    // rollback best effort
                    for (int r = 0; r < i; r++)
                        AddHandTile(tiles[r]);
                    for (int r = 0; r < slotList.Count; r++)
                        AddHandTile(slotList[r]);
                    slotList.Clear();
                    return false;
                }
                slotList.Add(id);
            }

            SortHandTiles();
            return true;
        }

        public bool TryPromoteToKanInSlot(int slotIndex, int tileId, out string reason)
        {
            reason = null;

            if (slotIndex < 0 || slotIndex > 3)
            {
                reason = "Invalid slot";
                return false;
            }
            if (_slotFixed[slotIndex])
            {
                reason = "Slot is fixed";
                return false;
            }
            if (Pool == null)
            {
                reason = "TilePool is null";
                return false;
            }

            if (!TryReplaceSlotWithTilesFromHand(slotIndex, new[] { tileId, tileId, tileId, tileId }, out reason))
                return false;

            _slotFixed[slotIndex] = true;

            if (!Pool.TryDrawRandom(_ => true, out var rinshanTileId))
            {
                return true;
            }

            AddHandTile(rinshanTileId);
            MarkRinshanTile(rinshanTileId);
            SortHandTiles();
            return true;
        }

        private void MarkRinshanTile(int tileId)
        {
            RinshanTileId = tileId;
            RinshanTileOccurrence = CountOf(tileId);
        }

        private void OnHandTileRemoved(int tileId)
        {
            if (RinshanTileId == 0 || tileId != RinshanTileId)
                return;

            int remain = CountOf(tileId);
            if (remain <= 0)
            {
                ClearRinshanState();
                return;
            }

            if (RinshanTileOccurrence > remain)
                RinshanTileOccurrence = remain;
        }

        private void ClearRinshanState()
        {
            RinshanTileId = 0;
            RinshanTileOccurrence = 0;
        }

    }
}
