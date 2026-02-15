using System;
using System.Collections.Generic;
using UnityEngine;

namespace FourMelds.Cards
{
    [CreateAssetMenu(menuName = "FourMelds/Cards/Card Authoring", fileName = "CardAuthoring_")]
    public sealed class CardAuthoringAsset : ScriptableObject
    {
        [Header("Identity")]
        public string cardId = "card.new";
        public string cardName = "새 카드";

        [Header("Visual")]
        public Sprite cardImage;
        [Range(0, 3)] public int rarity = 0;
        [Range(0, 4)] public int suitTheme = 0;

        [Header("Text")]
        public bool useAutoName = false;
        public bool useAutoDescription = true;
        [TextArea(2, 4)] public string manualName;
        [TextArea(3, 8)] public string manualDescription;

        [Header("Effects (Order Matters)")]
        public List<CardEffectTemplate> effects = new List<CardEffectTemplate>();

        public string GetResolvedName()
        {
            if (!useAutoName && !string.IsNullOrWhiteSpace(manualName))
                return manualName.Trim();

            if (!string.IsNullOrWhiteSpace(cardName))
                return cardName.Trim();

            return CardEffectTextComposer.BuildName(effects);
        }

        public string GetResolvedDescription()
        {
            if (!useAutoDescription && !string.IsNullOrWhiteSpace(manualDescription))
                return manualDescription.Trim();

            return CardEffectTextComposer.BuildDescription(effects);
        }

        public CardDefinition ToPrimaryDefinition()
        {
            var def = new CardDefinition
            {
                id = string.IsNullOrWhiteSpace(cardId) ? name : cardId.Trim(),
                name = GetResolvedName(),
                description = GetResolvedDescription(),
                rarity = Mathf.Clamp(rarity, 0, 3),
                suitTheme = Mathf.Clamp(suitTheme, 0, 4)
            };

            if (effects == null || effects.Count == 0 || effects[0] == null)
                return def;

            var primary = effects[0];
            def.action = primary.action == CardEffectActionType.Exchange ? "exchange" : "draw";
            def.suit = SuitToLegacy(primary.suit);
            def.count = Mathf.Max(1, primary.count);
            def.minRank = primary.minRank;
            def.maxRank = primary.maxRank;
            def.preferHonorFirst = primary.preferHonorFirst;
            return def;
        }

        private static string SuitToLegacy(CardEffectSuitType suit)
        {
            return suit switch
            {
                CardEffectSuitType.Manzu => "manzu",
                CardEffectSuitType.Pinzu => "pinzu",
                CardEffectSuitType.Souzu => "souzu",
                CardEffectSuitType.Honor => "honor",
                CardEffectSuitType.Number => "number",
                _ => "random"
            };
        }
    }
}
