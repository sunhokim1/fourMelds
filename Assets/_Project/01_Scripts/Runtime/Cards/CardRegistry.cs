using System.Collections.Generic;

namespace FourMelds.Cards
{
    public static class CardRegistry
    {
        // Day7: 하드코딩 테스트 카드
        public static readonly IReadOnlyList<ICardEffect> DefaultCards = new ICardEffect[]
        {
            new Card_DrawRandom5(),
            new Card_DrawManzu3(),
            new Card_DrawHonor1(),
            new Card_Exchange2(),
            new Card_DrawTanyaoLike4(),
        };
    }
}
