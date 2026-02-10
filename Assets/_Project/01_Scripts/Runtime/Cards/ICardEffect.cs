namespace FourMelds.Cards
{
    public interface ICardEffect
    {
        string Id { get; }
        string Name { get; }
        void Apply(CardContext ctx);
    }
}
