using System;

namespace FourMelds.Cards
{
    public enum CardEffectActionType
    {
        Tsumo = 0,
        Exchange = 1
    }

    public enum CardEffectSuitType
    {
        Manzu = 0,
        Pinzu = 1,
        Souzu = 2,
        Honor = 3,
        Random = 4,
        Number = 5
    }

    [Serializable]
    public sealed class CardEffectTemplate
    {
        public CardEffectActionType action = CardEffectActionType.Tsumo;
        public CardEffectSuitType suit = CardEffectSuitType.Random;
        public int count = 1;

        // suit=Number일 때만 사용
        public int minRank = 2;
        public int maxRank = 8;

        // suit=Random && action=Tsumo일 때만 사용
        public bool preferHonorFirst;
    }
}
