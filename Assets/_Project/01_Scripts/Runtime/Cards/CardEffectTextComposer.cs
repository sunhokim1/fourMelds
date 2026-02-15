using System.Collections.Generic;
using System.Text;

namespace FourMelds.Cards
{
    public static class CardEffectTextComposer
    {
        public static string BuildDescription(IReadOnlyList<CardEffectTemplate> effects)
        {
            if (effects == null || effects.Count == 0)
                return "효과 없음";

            var sb = new StringBuilder(96);
            for (int i = 0; i < effects.Count; i++)
            {
                var line = BuildEffectLine(effects[i]);
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(line);
            }

            return sb.Length > 0 ? sb.ToString() : "효과 없음";
        }

        public static string BuildName(IReadOnlyList<CardEffectTemplate> effects)
        {
            if (effects == null || effects.Count == 0)
                return "카드";

            var first = effects[0];
            int count = ClampCount(first?.count ?? 1);
            string baseName = first?.action switch
            {
                CardEffectActionType.Tsumo => $"{SuitLabel(first.suit)} {count}장",
                CardEffectActionType.Exchange => $"{count}장 교환",
                _ => "카드"
            };

            if (effects.Count <= 1)
                return baseName;

            return $"{baseName} +{effects.Count - 1}";
        }

        public static string BuildEffectLine(CardEffectTemplate effect)
        {
            if (effect == null)
                return string.Empty;

            int count = ClampCount(effect.count);
            switch (effect.action)
            {
                case CardEffectActionType.Tsumo:
                    if (effect.suit == CardEffectSuitType.Number)
                    {
                        int min = effect.minRank > 0 ? effect.minRank : 2;
                        int max = effect.maxRank > 0 ? effect.maxRank : 8;
                        if (min > max)
                        {
                            int t = min;
                            min = max;
                            max = t;
                        }

                        return $"{min}~{max} 범위의 만수·통수·삭수 패 중 랜덤으로 {count}장 쯔모합니다.";
                    }

                    if (effect.suit == CardEffectSuitType.Random)
                    {
                        if (effect.preferHonorFirst)
                            return $"랜덤 패를 {count}장 쯔모합니다. 이 중 1장은 자패 우선으로 보정됩니다.";
                        return $"랜덤 패를 {count}장 쯔모합니다.";
                    }

                    return $"{SuitLabel(effect.suit)} 패 중 랜덤으로 {count}장 쯔모합니다.";

                case CardEffectActionType.Exchange:
                    if (effect.suit == CardEffectSuitType.Random || effect.suit == CardEffectSuitType.Number)
                        return $"손패 {count}장을 랜덤으로 교환합니다.";
                    return $"손패의 {SuitLabel(effect.suit)} 패 {count}장을 교환합니다.";

                default:
                    return string.Empty;
            }
        }

        private static int ClampCount(int count)
        {
            return count <= 0 ? 1 : count;
        }

        private static string SuitLabel(CardEffectSuitType suit)
        {
            return suit switch
            {
                CardEffectSuitType.Manzu => "만수",
                CardEffectSuitType.Pinzu => "통수",
                CardEffectSuitType.Souzu => "삭수",
                CardEffectSuitType.Honor => "자패",
                CardEffectSuitType.Random => "랜덤",
                CardEffectSuitType.Number => "수패",
                _ => "패"
            };
        }
    }
}
