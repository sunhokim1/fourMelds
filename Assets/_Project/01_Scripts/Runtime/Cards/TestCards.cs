using Project.Core.Tiles;

namespace FourMelds.Cards
{
    public sealed class Card_DrawRandom5 : ICardEffect
    {
        public string Id => "draw.random5";
        public string Name => "랜덤 5장";

        public void Apply(CardContext ctx)
        {
            int drawn = 0;

            // Draw cards should visibly include honors more often.
            if (ctx.Pool.TryDrawRandom(id => id / 100 == 4, out var honor))
            {
                ctx.Turn.AddHandTile(honor);
                drawn++;
            }

            for (int i = drawn; i < 5; i++)
            {
                if (!ctx.Pool.TryDrawRandom(_ => true, out var t)) break;
                ctx.Turn.AddHandTile(t);
            }
        }
    }

    public sealed class Card_DrawManzu3 : ICardEffect
    {
        public string Id => "draw.manzu3";
        public string Name => "만수 3장";

        public void Apply(CardContext ctx)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!ctx.Pool.TryDrawRandom(id => id / 100 == 1, out var t)) break;
                ctx.Turn.AddHandTile(t);
            }
        }
    }

    public sealed class Card_DrawHonor1 : ICardEffect
    {
        public string Id => "draw.honor1";
        public string Name => "자패 1장";

        public void Apply(CardContext ctx)
        {
            if (ctx.Pool.TryDrawRandom(id => id / 100 == 4, out var t))
                ctx.Turn.AddHandTile(t);
        }
    }

    public sealed class Card_Exchange2 : ICardEffect
    {
        public string Id => "exchange.2";
        public string Name => "2장 교환";

        public void Apply(CardContext ctx)
        {
            var hand = ctx.Turn.HandTiles;
            int removeCount = hand.Count >= 2 ? 2 : hand.Count;

            for (int i = 0; i < removeCount; i++)
            {
                int id = ctx.Turn.HandTiles[0];
                ctx.Turn.TryRemoveTile(id);
            }

            int redrawn = 0;
            if (removeCount > 0 && ctx.Pool.TryDrawRandom(id => id / 100 == 4, out var honor))
            {
                ctx.Turn.AddHandTile(honor);
                redrawn++;
            }

            for (int i = redrawn; i < removeCount; i++)
            {
                if (!ctx.Pool.TryDrawRandom(_ => true, out var t)) break;
                ctx.Turn.AddHandTile(t);
            }
        }
    }

    public sealed class Card_DrawTanyaoLike4 : ICardEffect
    {
        public string Id => "draw.tanyao4";
        public string Name => "2~8 수패 4장";

        public void Apply(CardContext ctx)
        {
            bool Pred(int id)
            {
                int group = id / 100;
                int rank = id % 100;
                if (group == 4) return false;
                return rank >= 2 && rank <= 8;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!ctx.Pool.TryDrawRandom(Pred, out var t)) break;
                ctx.Turn.AddHandTile(t);
            }
        }
    }
}
