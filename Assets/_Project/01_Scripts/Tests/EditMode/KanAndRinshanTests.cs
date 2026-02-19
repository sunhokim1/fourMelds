using NUnit.Framework;
using FourMelds.Combat;
using FourMelds.Combat.TurnIntegration;
using Project.Core.Tiles;
using Project.Core.Turn;

public class KanAndRinshanTests
{
    [Test]
    public void PromoteToKan_FixesSlot_AndDrawsRinshanTile()
    {
        var state = new TurnState(new[] { 101, 101, 101, 101 });
        state.SetPool(new TilePool(new[] { 204 }, copiesPerTile: 1, seed: 1));

        var ok = state.TryPromoteToKanInSlot(0, 101, out var reason);

        Assert.That(ok, Is.True, reason);
        Assert.That(state.IsSlotFixed(0), Is.True);
        Assert.That(state.GetSlotTiles(0).Count, Is.EqualTo(4));
        Assert.That(state.RinshanTileId, Is.EqualTo(204));
        Assert.That(state.CountOf(204), Is.EqualTo(1));
    }

    [Test]
    public void RinshanYaku_RequiresFourCompleteMelds_AndRinshanTileUsage()
    {
        var state = new TurnState(new[]
        {
            101, 101, 101, 101,
            102, 102, 102,
            103, 103, 103,
            202, 203
        });
        state.SetPool(new TilePool(new[] { 204 }, copiesPerTile: 1, seed: 1));

        Assert.That(state.TryPromoteToKanInSlot(0, 101, out _), Is.True);
        Assert.That(state.TryReplaceSlotWithTilesFromHand(1, new[] { 102, 102, 102 }, out _), Is.True);
        Assert.That(state.TryReplaceSlotWithTilesFromHand(2, new[] { 103, 103, 103 }, out _), Is.True);
        Assert.That(state.TryReplaceSlotWithTilesFromHand(3, new[] { 202, 203, 204 }, out _), Is.True);

        var ctx = new TurnAttackContextBuilder().Build(state, new CombatState(50, 50));
        var yaku = new Yaku_RinshanKaihou();

        Assert.That(ctx.MeldCount, Is.EqualTo(4));
        Assert.That(yaku.IsActive(in ctx), Is.True);
    }
}
