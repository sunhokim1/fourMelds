using Project.Core.Turn;

namespace FourMelds.Cards
{
    /// <summary>
    /// 카드 실행의 단일 진입점
    /// UI / 입력 방식과 무관하게 항상 여기만 탄다.
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
    }
}
