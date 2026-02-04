using FourMelds.Core.Suits;
using System.Collections.Generic;

namespace Project.Core.Tiles
{
    public static class MeldCandidateCalculator
    {
        // 타일 하나를 기준으로 가능한 슌쯔 후보(123 / 234 / ...) "형태"만 반환
        public static List<MeldCandidate> GetShuntsuShapes(int tileValue)
        {
            var t = new TileId(tileValue);
            var result = new List<MeldCandidate>();

            if (!t.IsNumberSuit) return result;

            // 예: 5면 (345), (456), (567) 형태가 가능
            // 단, Rank 범위를 벗어나면 제외
            AddIfValid(result, t.Suit, t.Rank - 2); // (r-2, r-1, r)
            AddIfValid(result, t.Suit, t.Rank - 1); // (r-1, r, r+1)
            AddIfValid(result, t.Suit, t.Rank);     // (r, r+1, r+2)

            return result;
        }

        private static void AddIfValid(List<MeldCandidate> list, MahjongSuit suit, int startRank)
        {
            int a = startRank;
            int b = startRank + 1;
            int c = startRank + 2;

            if (a < 1 || c > 9) return;

            list.Add(new MeldCandidate(
                TileId.From(suit, a).Value,
                TileId.From(suit, b).Value,
                TileId.From(suit, c).Value
            ));
        }
    }
}