using System;
using System.Collections.Generic;
using UnityEngine;

namespace FourMelds.Cards
{
    public static class CardRegistry
    {
        private static readonly Dictionary<string, ICardEffect> _effectMap = new Dictionary<string, ICardEffect>(StringComparer.Ordinal)
        {
            ["draw.random5"] = new Card_DrawRandom5(),
            ["draw.manzu3"] = new Card_DrawManzu3(),
            ["draw.honor1"] = new Card_DrawHonor1(),
            ["exchange.2"] = new Card_Exchange2(),
            ["draw.tanyao4"] = new Card_DrawTanyaoLike4(),
        };

        private static IReadOnlyList<CardDefinition> _definitions;
        private static IReadOnlyList<ICardEffect> _cards;

        public static IReadOnlyList<CardDefinition> Definitions => _definitions ??= LoadDefinitions();
        public static IReadOnlyList<ICardEffect> DefaultCards => _cards ??= BuildCards(Definitions);

        public static bool TryGetDefinition(int index, out CardDefinition definition)
        {
            var defs = Definitions;
            if (index >= 0 && index < defs.Count)
            {
                definition = defs[index];
                return true;
            }

            definition = null;
            return false;
        }

        private static IReadOnlyList<ICardEffect> BuildCards(IReadOnlyList<CardDefinition> definitions)
        {
            var list = new List<ICardEffect>(definitions.Count);
            for (int i = 0; i < definitions.Count; i++)
            {
                var def = definitions[i];
                if (def == null || string.IsNullOrWhiteSpace(def.id))
                {
                    list.Add(new UnknownCardEffect($"unknown.{i}", $"Card {i}"));
                    continue;
                }

                if (_effectMap.TryGetValue(def.id, out var effect) && effect != null)
                {
                    list.Add(effect);
                    continue;
                }

                list.Add(new UnknownCardEffect(def.id, string.IsNullOrWhiteSpace(def.name) ? def.id : def.name));
            }

            return list;
        }

        private static IReadOnlyList<CardDefinition> LoadDefinitions()
        {
            // 1) Legacy aggregated file (order baseline)
            var defs = LoadFromLegacyDefaultFile();

            // 2) Split files (id-based override + append)
            MergeSplitCardFiles(defs);
            if (defs.Count > 0)
                return defs;

            Debug.LogWarning("[CardRegistry] No card definitions found in Resources. Falling back to hardcoded defaults.");
            return BuildFallbackDefinitions();
        }

        private static List<CardDefinition> LoadFromLegacyDefaultFile()
        {
            var defs = new List<CardDefinition>();
            var text = Resources.Load<TextAsset>("CardDefinitions/default_cards");
            if (text == null || string.IsNullOrWhiteSpace(text.text))
                return defs;

            try
            {
                var wrapped = JsonUtility.FromJson<CardDefinitionListWrapper>(text.text);
                if (wrapped?.cards == null || wrapped.cards.Length == 0)
                    return defs;

                for (int i = 0; i < wrapped.cards.Length; i++)
                {
                    var d = wrapped.cards[i];
                    if (d == null || string.IsNullOrWhiteSpace(d.id))
                        continue;
                    NormalizeDefinition(d);
                    defs.Add(d);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[CardRegistry] Failed to parse default_cards.json: {ex.Message}");
            }

            return defs;
        }

        private static void MergeSplitCardFiles(List<CardDefinition> defs)
        {
            var assets = Resources.LoadAll<TextAsset>("CardDefinitions/Cards");
            if (assets == null || assets.Length == 0)
                return;

            Array.Sort(assets, (a, b) => string.CompareOrdinal(a.name, b.name));
            for (int i = 0; i < assets.Length; i++)
            {
                var a = assets[i];
                if (a == null || string.IsNullOrWhiteSpace(a.text))
                    continue;

                if (!TryParseSingleCard(a.text, out var parsed) || parsed == null || string.IsNullOrWhiteSpace(parsed.id))
                {
                    Debug.LogWarning($"[CardRegistry] Invalid split card JSON: {a.name}");
                    continue;
                }

                NormalizeDefinition(parsed);
                int existing = defs.FindIndex(d => d != null && string.Equals(d.id, parsed.id, StringComparison.Ordinal));
                if (existing >= 0)
                    defs[existing] = parsed;  // override legacy with split file
                else
                    defs.Add(parsed);         // append new card definitions
            }
        }

        private static bool TryParseSingleCard(string json, out CardDefinition card)
        {
            card = null;
            if (string.IsNullOrWhiteSpace(json))
                return false;

            try
            {
                card = JsonUtility.FromJson<CardDefinition>(json);
                return card != null;
            }
            catch
            {
                return false;
            }
        }

        private static IReadOnlyList<CardDefinition> BuildFallbackDefinitions()
        {
            var defs = new List<CardDefinition>
            {
                new CardDefinition { id = "draw.random5", rarity = 0, suitTheme = 0, action = "draw", suit = "random", count = 5, preferHonorFirst = true },
                new CardDefinition { id = "draw.manzu3", rarity = 0, suitTheme = 1, action = "draw", suit = "manzu", count = 3 },
                new CardDefinition { id = "draw.honor1", rarity = 0, suitTheme = 4, action = "draw", suit = "honor", count = 1 },
                new CardDefinition { id = "exchange.2", rarity = 0, suitTheme = 0, action = "exchange", suit = "random", count = 2 },
                new CardDefinition { id = "draw.tanyao4", rarity = 0, suitTheme = 0, action = "draw", suit = "number", count = 4, minRank = 2, maxRank = 8 },
            };

            for (int i = 0; i < defs.Count; i++)
                NormalizeDefinition(defs[i]);

            return defs;
        }

        private static void NormalizeDefinition(CardDefinition def)
        {
            if (def == null)
                return;

            if (def.count <= 0)
                def.count = 1;
            if (string.IsNullOrWhiteSpace(def.name))
                def.name = CardTextComposer.BuildName(def);
            if (string.IsNullOrWhiteSpace(def.description))
                def.description = CardTextComposer.BuildDescription(def);
        }

        [Serializable]
        private sealed class CardDefinitionListWrapper
        {
            public CardDefinition[] cards;
        }

        private sealed class UnknownCardEffect : ICardEffect
        {
            public string Id { get; }
            public string Name { get; }

            public UnknownCardEffect(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public void Apply(CardContext ctx)
            {
                Debug.LogWarning($"[CARD] Unknown card effect id={Id}");
            }
        }
    }
}
