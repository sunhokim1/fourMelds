using System.Collections.Generic;

namespace Project.Core.Tiles
{
    /// <summary>
    /// 게임에서 사용하는 전체 타일 ID 카탈로그.
    /// - 숫자패: (suit*100 + rank)  suit: 1=Manzu,2=Souzu,3=Pinzu  rank:1~9
    /// - 자패: suit=4(Honor) + rank: 1~7  (동남서북백발중)
    /// </summary>
    public static class TileCatalog
    {
        // Honor ranks (권장 고정)
        public const int HONOR_EAST = 401;
        public const int HONOR_SOUTH = 402;
        public const int HONOR_WEST = 403;
        public const int HONOR_NORTH = 404;
        public const int HONOR_WHITE = 405; // 백
        public const int HONOR_GREEN = 406; // 발
        public const int HONOR_RED = 407; // 중

        public static IReadOnlyList<int> AllTileIds => _allTileIds;
        private static readonly List<int> _allTileIds = BuildAll();

        private static List<int> BuildAll()
        {
            var list = new List<int>(34);

            // 1=Manzu, 2=Souzu, 3=Pinzu
            for (int suit = 1; suit <= 3; suit++)
            {
                for (int rank = 1; rank <= 9; rank++)
                    list.Add((suit * 100) + rank);
            }

            // Honors: 401~407
            for (int rank = 1; rank <= 7; rank++)
                list.Add((4 * 100) + rank);

            return list;
        }
    }
}
