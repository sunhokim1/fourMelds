namespace FourMelds.Combat.TurnIntegration
{
    public sealed class TurnEndAttackExecutor
    {
        private readonly DamagePipeline _pipeline;

        public TurnEndAttackExecutor(DamagePipeline pipeline)
        {
            _pipeline = pipeline;
        }

        public AttackResult Execute(in AttackContext ctx)
        {
            return _pipeline.Execute(ctx);
        }
    }
}
