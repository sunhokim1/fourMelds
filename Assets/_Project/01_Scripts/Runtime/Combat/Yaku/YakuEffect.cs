using Project.Core.Melds;
using Project.Core.Tiles;

namespace FourMelds.Combat
{
    public sealed class Yaku_Tanyao : IYakuEffect
    {
        public string Id => "tanyao";

        public bool IsActive(in AttackContext ctx)
        {
            // 머리 없으면 실패로 처리 (원하면 true로 바꿔도 됨)
            if (!ctx.HasHead) return false;

            // ✅ 머리 검사 (1/9/자패면 탕야오 실패)
            {
                TileId head = new TileId(ctx.HeadTileId);
                if (head.IsHonor) return false;
                if (head.Rank < 2 || head.Rank > 8) return false;
            }

            // ✅ 몸통 타일 검사
            var melds = ctx.Melds;
            if (melds == null || melds.Count == 0) return false;

            for (int i = 0; i < melds.Count; i++)
            {
                var tiles = melds[i].Tiles;
                if (tiles == null || tiles.Count == 0) return false;

                for (int t = 0; t < tiles.Count; t++)
                {
                    TileId tile = new TileId(tiles[t]);
                    if (tile.IsHonor) return false;
                    if (tile.Rank < 2 || tile.Rank > 8) return false;
                }

            }

            return true;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.GlobalMult *= 1.20f;
        }
    }

    public sealed class Yaku_Toitoi : IYakuEffect
    {
        public string Id => "toitoi";

        public bool IsActive(in AttackContext ctx)
        {
            var melds = ctx.Melds;
            if (melds == null || melds.Count == 0) return false;

            for (int i = 0; i < melds.Count; i++)
            {
                var t = melds[i].Type;
                if (t != MeldType.Koutsu && t != MeldType.Kantsu)
                    return false;
            }
            return true;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.GlobalMult *= 1.35f;
        }
    }

    public sealed class Yaku_Sanankou : IYakuEffect
    {
        public string Id => "sanankou";

        public bool IsActive(in AttackContext ctx)
        {
            return CountAnkou(in ctx) >= 3;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.GlobalMult *= 1.25f;
        }

        private static int CountAnkou(in AttackContext ctx)
        {
            var melds = ctx.Melds;
            if (melds == null || melds.Count == 0) return 0;

            int c = 0;
            for (int i = 0; i < melds.Count; i++)
            {
                var m = melds[i];
                if ((m.Type == MeldType.Koutsu || m.Type == MeldType.Kantsu) && m.IsFixed == false)
                    c++;
            }
            return c;
        }
    }

    public sealed class Yaku_Suankou : IYakuEffect
    {
        public string Id => "suankou";

        public bool IsActive(in AttackContext ctx)
        {
            return CountAnkou(in ctx) >= 4;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.GlobalMult *= 1.45f;
        }

        private static int CountAnkou(in AttackContext ctx)
        {
            var melds = ctx.Melds;
            if (melds == null || melds.Count == 0) return 0;

            int c = 0;
            for (int i = 0; i < melds.Count; i++)
            {
                var m = melds[i];
                if ((m.Type == MeldType.Koutsu || m.Type == MeldType.Kantsu) && m.IsFixed == false)
                    c++;
            }
            return c;
        }
    }

    public sealed class Yaku_RinshanKaihou : IYakuEffect
    {
        public string Id => "rinshan";

        public bool IsActive(in AttackContext ctx)
        {
            // 린샹쯔모가 있었고, 4개 몸통이 완성되어 있으며, 해당 타일이 몸통에 실제 사용된 경우에만 성립
            if (!ctx.HasRinshanTile)
                return false;
            if (ctx.MeldCount < 4)
                return false;
            if (!ctx.UsesRinshanTileInMelds)
                return false;
            return true;
        }

        public void Apply(in AttackContext ctx, AttackMutableState state)
        {
            state.GlobalMult *= 1.30f;
        }
    }
}
