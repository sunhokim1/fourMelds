// Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnState.cs

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

        public TurnState(IEnumerable<int> initialTiles)
        {
            _handTiles = new List<int>(initialTiles);
        }

        public void SetPhase(TurnPhase phase) => Phase = phase;
        public int HeadTileId { get; private set; } = 0; // 0이면 아직 없음
        public void SetHeadTile(int tileId) => HeadTileId = tileId;
        public TilePool Pool { get; private set; }
        public void AddHandTile(int tileId) => _handTiles.Add(tileId);


        public void AdvanceTurn()
        {
            TurnIndex++;
            Phase = TurnPhase.Draw; // RoundStart 같은 옛 값 제거
        }

        public void CleanupForNextTurn()
        {
            // 0) 이번 턴 소멸(규칙)
            ClearAllHandTiles();
            ClearAllMelds(resetMeldId: true);

            // 1) 턴 진행
            AdvanceTurn(); // TurnIndex++ / Phase=Draw

            // 2) 턴 단위 타일 풀 리셋 (각 타일 4장)
            Pool = new TilePool(TileCatalog.AllTileIds, copiesPerTile: 4);

            // 3) 머리(쌍) 뽑기: 같은 타일 2장 차감
            //    첫 장: TryDrawRandom이 1장 차감
            //    둘째 장: TryConsume로 1장 더 차감해서 "쌍" 완성
            if (Pool.TryDrawRandom(_ => true, out int head))
            {
                // 풀 초기 상태라 대부분 항상 성공(4장 중 2장 확보 가능)
                if (!Pool.TryConsume(head, 1))
                {
                    // 방어용(거의 발생 안 함)
                    HeadTileId = 0;
                }
                else
                {
                    HeadTileId = head;
                }
            }
            else
            {
                HeadTileId = 0;
            }

            // 4) 카드 시스템 전까지 임시 손패 드로우
            //    필요하면 숫자만 바꿔.
            const int initialHand = 8;
            for (int i = 0; i < initialHand; i++)
            {
                if (!Pool.TryDrawRandom(_ => true, out int t))
                    break;

                AddHandTile(t);
            }
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

        // --- 턴 정리용 (룰: 턴 끝나면 전부 소멸) ---

        public void ClearAllHandTiles() => _handTiles.Clear();

        public void ClearAllMelds(bool resetMeldId = true)
        {
            _melds.Clear();
            if (resetMeldId) _nextMeldId = 1;
        }

        public void Dev_SetHandTiles(IEnumerable<int> tiles)
        {
            _handTiles.Clear();
            _handTiles.AddRange(tiles);
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
    }
}
