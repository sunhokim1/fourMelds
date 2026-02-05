using System;
using System.Collections.Generic;

namespace Project.Core.Tiles
{
    /// <summary>
    /// 턴 단위 타일 재고(각 타일 최대 4장).
    /// 머리/랜덤드로우/교환 모두 여기서만 발생하게 해서 상한선을 보장한다.
    /// </summary>
    public sealed class TilePool
    {
        private readonly Dictionary<int, int> _remain = new();
        private readonly Random _rng;

        public TilePool(IEnumerable<int> allTileIds, int copiesPerTile = 4, int? seed = null)
        {
            if (copiesPerTile <= 0) throw new ArgumentOutOfRangeException(nameof(copiesPerTile));
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();

            foreach (var id in allTileIds)
                _remain[id] = copiesPerTile;
        }

        public int GetRemain(int tileId) => _remain.TryGetValue(tileId, out var v) ? v : 0;

        public bool TryConsume(int tileId, int count = 1)
        {
            if (count <= 0) return true;
            if (!_remain.TryGetValue(tileId, out var v)) return false;
            if (v < count) return false;

            _remain[tileId] = v - count;
            return true;
        }

        /// <summary>
        /// 조건을 만족하는 타일 중 재고>0인 것들에서 1장 랜덤 드로우.
        /// 성공 시 out tileId에 반환하고 재고 1 감소.
        /// </summary>
        public bool TryDrawRandom(Func<int, bool> predicate, out int tileId)
        {
            // 후보 수집 (성능은 나중에 최적화; 지금 규모에선 충분)
            var candidates = new List<int>();
            foreach (var kv in _remain)
            {
                if (kv.Value <= 0) continue;
                if (predicate != null && !predicate(kv.Key)) continue;
                candidates.Add(kv.Key);
            }

            if (candidates.Count == 0)
            {
                tileId = 0;
                return false;
            }

            tileId = candidates[_rng.Next(candidates.Count)];
            _remain[tileId] -= 1;
            return true;
        }
    }
}
