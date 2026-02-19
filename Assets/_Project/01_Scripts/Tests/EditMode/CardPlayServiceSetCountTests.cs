using NUnit.Framework;
using FourMelds.Cards;
using Project.Core.Turn;

public class CardPlayServiceSetCountTests
{
    [Test]
    public void TryApplySetTileCount_UpgradesTwoCopiesToThree()
    {
        var state = new TurnState(new[] { 202, 202, 105, 401 });

        var ok = CardPlayService.TryApplySetTileCount(state, 202, 3, out var reason);

        Assert.That(ok, Is.True, reason);
        Assert.That(state.CountOf(202), Is.EqualTo(3));
        Assert.That(state.HandTiles.Count, Is.EqualTo(5));
    }

    [Test]
    public void TryApplySetTileCount_DowngradesFourCopiesToThree()
    {
        var state = new TurnState(new[] { 302, 302, 302, 302, 107 });

        var ok = CardPlayService.TryApplySetTileCount(state, 302, 3, out var reason);

        Assert.That(ok, Is.True, reason);
        Assert.That(state.CountOf(302), Is.EqualTo(3));
        Assert.That(state.HandTiles.Count, Is.EqualTo(4));
    }
}
