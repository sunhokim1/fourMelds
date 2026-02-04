namespace FourMelds.Combat
{
    public sealed record PlayerStateSnapshot(
        int Hp,
        int MaxHp,
        int Armor,
        int Energy,
        int ComboCounter
    )
    {
        /// <summary>
        /// Day3+ 최소 생성용. HP만 주면 나머지는 안전한 기본값으로 채운다.
        /// </summary>
        public static PlayerStateSnapshot FromHp(int hp)
        {
            if (hp < 0) hp = 0;
            return new PlayerStateSnapshot(
                Hp: hp,
                MaxHp: hp,
                Armor: 0,
                Energy: 0,
                ComboCounter: 0
            );
        }

        /// <summary>
        /// 흔히 쓰는 초기화(게임 시작값 등)
        /// </summary>
        public static PlayerStateSnapshot Initial(int hp, int energy)
        {
            if (hp < 0) hp = 0;
            if (energy < 0) energy = 0;
            return new PlayerStateSnapshot(
                Hp: hp,
                MaxHp: hp,
                Armor: 0,
                Energy: energy,
                ComboCounter: 0
            );
        }
    }
}
