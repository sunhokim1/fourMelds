namespace FourMelds.Combat
{
    public readonly struct YakuMeta
    {
        public string DisplayName { get; }
        public int Order { get; }

        public YakuMeta(string displayName, int order)
        {
            DisplayName = displayName;
            Order = order;
        }
    }

    public static class YakuMetaDb
    {
        public static YakuMeta Get(string id) => id switch
        {
            // ✅ 약한 것 -> 강한 것(연출 “딱딱딱”용)
            "tanyao" => new YakuMeta("탕야오", 100),
            "toitoi" => new YakuMeta("또이또이", 200),
            "sanankou" => new YakuMeta("산앙커", 300),
            "suankou" => new YakuMeta("쓰앙커", 400),
            "rinshan" => new YakuMeta("영상개화", 500),

            _ => new YakuMeta(id, 9999)
        };
    }
}
