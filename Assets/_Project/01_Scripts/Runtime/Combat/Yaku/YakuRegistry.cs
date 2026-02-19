using System;

namespace FourMelds.Combat
{
    public static class YakuRegistry
    {
        // 지금은 하드코딩 (Day8 이후 SO/데이터로 교체)
        public static IYakuEffect[] CreateDefault()
        {
            return new IYakuEffect[]
            {
                new Yaku_Tanyao(),
                new Yaku_Toitoi(),
                new Yaku_Sanankou(),
                new Yaku_Suankou(),
                new Yaku_RinshanKaihou()
            };
        }
    }
}
