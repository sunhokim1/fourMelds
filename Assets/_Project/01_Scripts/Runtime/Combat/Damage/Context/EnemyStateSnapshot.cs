namespace FourMelds.Combat
{
    public sealed record EnemyStateSnapshot(
        int Hp,
        int MaxHp,
        int Armor,
        int Level
    )
    {
        /// <summary>
        /// Day3+ 최소 생성용. HP만 주면 나머지는 안전한 기본값으로 채운다.
        /// </summary>
        public static EnemyStateSnapshot FromHp(int hp)
        {
            if (hp < 0) hp = 0;
            return new EnemyStateSnapshot(
                Hp: hp,
                MaxHp: hp,
                Armor: 0,
                Level: 1
            );
        }
    }
}
