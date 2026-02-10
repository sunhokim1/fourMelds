namespace FourMelds.Combat
{
    /// <summary>
    /// 데미지 계산의 한 단계.
    /// 순서대로 호출되며 AttackMutableState를 수정한다.
    /// </summary>
    public interface IDamageStep
    {
        string Id { get; }
        void Apply(in AttackContext ctx, AttackMutableState state);
    }
}