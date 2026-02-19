using System;

namespace FourMelds.Cards
{
    [Serializable]
    public sealed class CardDefinition
    {
        public string id;
        public string name;
        public string description;
        public int rarity;
        public int suitTheme;
        public string action;      // draw, exchange, setcount
        public string suit;        // manzu, souzu, pinzu, honor, random, number
        public int count;          // primary amount
        public int minRank;        // optional for suit=number
        public int maxRank;        // optional for suit=number
        public bool preferHonorFirst;
    }
}
