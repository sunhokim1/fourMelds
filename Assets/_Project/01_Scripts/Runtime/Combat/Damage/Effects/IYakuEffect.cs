namespace FourMelds.Combat
{
    public interface IYakuEffect
    {
        string Id { get; }
        bool IsActive(in AttackContext ctx);
        void Apply(in AttackContext ctx, AttackMutableState state);
    }
}