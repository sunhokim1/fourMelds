using Project.Core.Turn;
using Project.Core.Tiles;

namespace FourMelds.Cards
{
    public readonly struct CardContext
    {
        public TurnState Turn { get; }
        public TilePool Pool { get; }

        public CardContext(TurnState turn, TilePool pool)
        {
            Turn = turn;
            Pool = pool;
        }
    }
}
