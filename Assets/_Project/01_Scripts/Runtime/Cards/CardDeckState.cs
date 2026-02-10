using System;
using System.Collections.Generic;

namespace FourMelds.Cards
{
    /// <summary>
    /// Slay-the-Spire style deck state:
    /// draw pile, discard pile, and current hand.
    /// </summary>
    public sealed class CardDeckState
    {
        private readonly List<int> _drawPile = new();
        private readonly List<int> _discardPile = new();
        private readonly List<int> _hand = new();
        private readonly Random _rng;

        public IReadOnlyList<int> HandCards => _hand;
        public int DrawPileCount => _drawPile.Count;
        public int DiscardPileCount => _discardPile.Count;

        public CardDeckState(int? seed = null)
        {
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public void SetDeck(IEnumerable<int> deckCardIndices)
        {
            _drawPile.Clear();
            _discardPile.Clear();
            _hand.Clear();

            if (deckCardIndices == null)
                return;

            foreach (var idx in deckCardIndices)
                _drawPile.Add(idx);

            Shuffle(_drawPile);
        }

        public void StartTurnDraw(int drawCount)
        {
            DiscardHand();
            DrawToHand(drawCount);
        }

        public void DiscardHand()
        {
            if (_hand.Count == 0)
                return;

            _discardPile.AddRange(_hand);
            _hand.Clear();
        }

        public bool TryGetHandCard(int handIndex, out int cardIndex)
        {
            if (handIndex < 0 || handIndex >= _hand.Count)
            {
                cardIndex = -1;
                return false;
            }

            cardIndex = _hand[handIndex];
            return true;
        }

        public bool TryPlayHandCard(int handIndex, out int cardIndex)
        {
            if (!TryGetHandCard(handIndex, out cardIndex))
                return false;

            _hand.RemoveAt(handIndex);
            _discardPile.Add(cardIndex);
            return true;
        }

        private void DrawToHand(int count)
        {
            if (count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                if (_drawPile.Count == 0)
                {
                    if (_discardPile.Count == 0)
                        break;

                    _drawPile.AddRange(_discardPile);
                    _discardPile.Clear();
                    Shuffle(_drawPile);
                }

                int top = _drawPile[_drawPile.Count - 1];
                _drawPile.RemoveAt(_drawPile.Count - 1);
                _hand.Add(top);
            }
        }

        private void Shuffle(List<int> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
