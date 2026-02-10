namespace FourMelds.Combat
{
    /// <summary>
    /// Yaku Id → 연출용 표시 이름 매핑
    /// (콘솔 로그 / UI 연출 공용)
    /// </summary>
    public static class YakuNames
    {
        public static string ToDisplay(string id) => id switch
        {
            "tanyao" => "탕야오",
            "toitoi" => "또이또이",
            "sanankou" => "산앙커",
            "suankou" => "쓰앙커",
            _ => id
        };
    }
}
