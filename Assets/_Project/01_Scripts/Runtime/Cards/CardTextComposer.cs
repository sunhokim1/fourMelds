using System;

namespace FourMelds.Cards
{
    public static class CardTextComposer
    {
        public static string BuildName(CardDefinition def)
        {
            if (def == null)
                return "카드";

            int count = Math.Max(1, def.count);
            switch (Safe(def.action))
            {
                case "draw":
                    return $"{SuitLabel(def)} {count}장";
                case "exchange":
                    return $"{count}장 교환";
                case "setcount":
                    return $"{count}장 복제";
                default:
                    return string.IsNullOrWhiteSpace(def.id) ? "카드" : def.id;
            }
        }

        public static string BuildDescription(CardDefinition def)
        {
            if (def == null)
                return "효과 없음";

            int count = Math.Max(1, def.count);
            switch (Safe(def.action))
            {
                case "draw":
                    if (Safe(def.suit) == "number")
                    {
                        int min = def.minRank > 0 ? def.minRank : 2;
                        int max = def.maxRank > 0 ? def.maxRank : 8;
                        return $"{min}~{max} 범위의 만수·통수·삭수 패 중 랜덤으로 {count}장 쯔모합니다.";
                    }

                    if (Safe(def.suit) == "random")
                    {
                        if (def.preferHonorFirst)
                            return $"랜덤 패를 {count}장 쯔모합니다. 이 중 1장은 자패 우선으로 보정됩니다.";
                        return $"랜덤 패를 {count}장 쯔모합니다.";
                    }

                    return $"{SuitLabel(def)} 패 중 랜덤으로 {count}장 쯔모합니다.";

                case "exchange":
                    return $"손패 {count}장을 랜덤으로 교환합니다. 교환 후 {count}장을 다시 쯔모합니다.";
                case "setcount":
                    return $"손패에서 패 종류 1개를 선택합니다. 선택한 패를 정확히 {count}장으로 맞춥니다.";

                default:
                    return "효과 없음";
            }
        }

        private static string SuitLabel(CardDefinition def)
        {
            switch (Safe(def.suit))
            {
                case "manzu": return "만수";
                case "souzu": return "삭수";
                case "pinzu": return "통수";
                case "honor": return "자패";
                case "random": return "랜덤";
                case "number": return "수패";
                default: return "패";
            }
        }

        private static string Safe(string s) => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim().ToLowerInvariant();
    }
}
