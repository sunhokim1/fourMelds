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
        public bool TryDrawRandom(System.Func<int, bool> predicate, out int tileId)
        {
            // 1) 조건을 만족하는 남은 장수 총합 계산
            int total = 0;
            foreach (var kv in _remain)
            {
                int id = kv.Key;
                int remain = kv.Value;
                if (remain <= 0) continue;
                if (predicate != null && !predicate(id)) continue;
                total += remain;
            }

            if (total <= 0)
            {
                tileId = 0;
                return false;
            }

            // 2) [0, total)에서 랜덤 인덱스 선택
            int roll = _rng.Next(total);

            // 3) 누적합으로 어느 타일이 선택되는지 결정
            foreach (var kv in _remain)
            {
                int id = kv.Key;
                int remain = kv.Value;
                if (remain <= 0) continue;
                if (predicate != null && !predicate(id)) continue;

                roll -= remain;
                if (roll < 0)
                {
                    tileId = id;
                    _remain[id] = remain - 1;
                    return true;
                }
            }

            // 논리상 도달하지 않음(가드)
            tileId = 0;
            return false;
        }

    }
}
