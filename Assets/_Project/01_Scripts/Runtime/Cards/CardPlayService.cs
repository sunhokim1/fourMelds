using System;
using System.Collections.Generic;
using Project.Core.Turn;

namespace FourMelds.Cards
{
    /// <summary>
    /// Card play service.
    /// UI/input only dispatches and this class executes card effects.
    /// </summary>
    public static class CardPlayService
    {
        public static bool TryPlay(int cardIndex, TurnState state, out string failReason)
        {
            failReason = null;

            if (state == null)
            {
                failReason = "TurnState is null";
                return false;
            }

            if (state.Pool == null)
            {
                failReason = "TilePool is null";
                return false;
            }

            var cards = CardRegistry.DefaultCards;
            if (cardIndex < 0 || cardIndex >= cards.Count)
            {
                failReason = $"Card index out of range: {cardIndex}";
                return false;
            }

            var card = cards[cardIndex];
            if (card == null)
            {
                failReason = "Card is null";
                return false;
            }

            card.Apply(new CardContext(state, state.Pool));
            return true;
        }

        public static bool TryApplyExchange(
            TurnState state,
            IReadOnlyList<int> selectedTileIds,
            CardDefinition definition,
            out string failReason)
        {
            failReason = null;

            if (state == null)
            {
                failReason = "TurnState is null";
                return false;
            }

            if (state.Pool == null)
            {
                failReason = "TilePool is null";
                return false;
            }

            if (selectedTileIds == null || selectedTileIds.Count == 0)
            {
                failReason = "No selected tiles";
                return false;
            }

            var removed = new List<int>(selectedTileIds.Count);
            for (int i = 0; i < selectedTileIds.Count; i++)
            {
                int tileId = selectedTileIds[i];
                if (!state.TryRemoveTile(tileId))
                {
                    for (int r = 0; r < removed.Count; r++)
                        state.AddHandTile(removed[r]);
                    state.SortHandTiles();
                    failReason = $"Tile not in hand: {tileId}";
                    return false;
                }

                removed.Add(tileId);
            }

            // Keep legacy behavior: exchange attempts to draw one honor first.
            int drawn = 0;
            if (state.Pool.TryDrawRandom(id => id / 100 == 4, out var honor))
            {
                state.AddHandTile(honor);
                drawn++;
            }

            for (int i = drawn; i < selectedTileIds.Count; i++)
            {
                if (!state.Pool.TryDrawRandom(BuildPredicate(definition), out var t))
                    break;
                state.AddHandTile(t);
            }

            state.SortHandTiles();
            return true;
        }

        private static Func<int, bool> BuildPredicate(CardDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.suit))
                return _ => true;

            string suit = definition.suit.Trim().ToLowerInvariant();
            if (suit == "manzu")
                return id => id / 100 == 1;
            if (suit == "souzu")
                return id => id / 100 == 2;
            if (suit == "pinzu")
                return id => id / 100 == 3;
            if (suit == "honor")
                return id => id / 100 == 4;
            if (suit == "number")
            {
                int min = definition.minRank > 0 ? definition.minRank : 1;
                int max = definition.maxRank > 0 ? definition.maxRank : 9;
                return id =>
                {
                    int group = id / 100;
                    if (group == 4)
                        return false;
                    int rank = id % 100;
                    return rank >= min && rank <= max;
                };
            }

            return _ => true;
        }
    }
}
