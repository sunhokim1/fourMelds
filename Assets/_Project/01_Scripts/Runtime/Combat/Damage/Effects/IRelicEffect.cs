namespace FourMelds.Combat
{
    public interface IRelicEffect
    {
        string Id { get; }
        bool IsActive(in AttackContext ctx);
        void Apply(in AttackContext ctx, AttackMutableState state);
    }
}