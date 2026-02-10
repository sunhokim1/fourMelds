using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.Tiles
{
    [CreateAssetMenu(menuName = "FourMelds/Tile Sprite Set", fileName = "TileSpriteSet")]
    public sealed class TileSpriteSet : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public int TileId;
            public Sprite Sprite;
        }

        [SerializeField] private List<Entry> entries = new();

        private Dictionary<int, Sprite> _map;

        public bool TryGet(int tileId, out Sprite sprite)
        {
            EnsureBuilt();
            return _map.TryGetValue(tileId, out sprite) && sprite != null;
        }

        private void EnsureBuilt()
        {
            if (_map != null) return;
            _map = new Dictionary<int, Sprite>(entries.Count);
            for (int i = 0; i < entries.Count; i++)
                _map[entries[i].TileId] = entries[i].Sprite;
        }
    }
}
