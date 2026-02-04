// Assets/_Project/01_Scripts/Runtime/Core/Turn/TurnPhase.cs

namespace FourMelds.Core.Turn
{
    /// <summary>
    /// One full round consists of multiple phases.
    /// Player input is only allowed in Card / Build phases.
    /// </summary>
    public enum TurnPhase
    {
        /// <summary>
        /// Start of round.
        /// Draw cards, reset temporary states.
        /// </summary>
        RoundStart,

        /// <summary>
        /// Card usage phase.
        /// Cards generate / modify Mahjong tiles.
        /// </summary>
        Card,

        /// <summary>
        /// Build phase.
        /// Assemble Mahjong tiles into melds (Shuntsu / Koutsu / Kantsu).
        /// </summary>
        Build,

        /// <summary>
        /// Resolve phase.
        /// No player input.
        /// AttackContext -> DamagePipeline -> AttackResult.
        /// </summary>
        Resolve,

        /// <summary>
        /// Enemy action phase.
        /// Enemy performs attacks / skills.
        /// </summary>
        Enemy,

        /// <summary>
        /// Cleanup phase.
        /// Remove all tiles, discard unused cards,
        /// prepare for next round.
        /// </summary>
        Cleanup
    }
}
