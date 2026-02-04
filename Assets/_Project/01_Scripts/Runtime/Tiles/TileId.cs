using System;
using FourMelds.Core.Suits;

namespace Project.Core.Tiles
{
    public readonly struct TileId : IEquatable<TileId>
    {
        public int Value { get; }

        public MahjongSuit Suit => (MahjongSuit)(Value / 100);
        public int Rank => Value % 100;

        public TileId(int value)
        {
            Value = value;
        }

        public static TileId From(MahjongSuit suit, int rank)
            => new TileId(((int)suit * 100) + rank);

        public bool IsNumberSuit => Suit != MahjongSuit.Honor;
        public bool IsHonor => Suit == MahjongSuit.Honor;

        public override string ToString() => $"{Suit}:{Rank} ({Value})";
        public bool Equals(TileId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is TileId other && Equals(other);
        public override int GetHashCode() => Value;
    }
}