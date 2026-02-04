using System.Collections.Generic;

namespace FourMelds.Combat
{
    public sealed record AttackResult(
        int FinalDamage,
        IReadOnlyList<AttackSideEffect> SideEffects,
        IReadOnlyList<DamageLogEntry> LogEntries
    );
}