using System.Linq;
using NUnit.Framework;
using FourMelds.Core.Turn;
using Project.Core.Action.Execute;
using Project.Core.Action.Query;
using Project.Core.Tiles;
using Project.Core.Turn;
using Project.InputSystem;
using Project.UI.Models;

public class TurnFlowRegressionTests
{
    [Test]
    public void HonorTile_DoesNotOfferShuntsu_InActionQuery()
    {
        var query = new DummyActionQueryService();
        var request = new ActionRequest(
            ActionRequestType.OpenActionMenu,
            ActionTargetType.Tile,
            401);

        var snapshot = new TurnSnapshot(
            TurnPhase.Build,
            turnIndex: 1,
            handTiles: new[] { 401, 402, 403, 405 },
            meldIds: new int[0]);

        var menu = query.Query(request, snapshot);

        Assert.That(menu.Options.Any(o => o.Command == ActionCommandType.CreateShuntsu), Is.False);
    }

    [Test]
    public void HonorShuntsu_IsRejected_InExecutionService()
    {
        var exec = new DummyActionExecutionService();
        var state = new TurnState(new[] { 401, 402, 403, 405 });

        var cmd = new ActionCommand(
            ActionCommandType.CreateShuntsu,
            targetId: 0,
            payload: new MeldCandidate(401, 402, 403));

        var ok = exec.Execute(cmd, state, out var reason);

        Assert.That(ok, Is.False);
        Assert.That(reason, Is.EqualTo("Invalid shuntsu shape"));
        Assert.That(state.HandTiles.Count, Is.EqualTo(4));
        Assert.That(state.HandTiles.Contains(401), Is.True);
        Assert.That(state.HandTiles.Contains(402), Is.True);
        Assert.That(state.HandTiles.Contains(403), Is.True);
    }

    [Test]
    public void QuickMeldReplace_ReplacesSlotAndReturnsPreviousTilesToHand()
    {
        var state = new TurnState(new[] { 101, 102, 103, 201, 201, 201, 401 });
        state.SelectSlot(0);

        Assert.That(state.TryAddTileToSelectedSlot(101, out _), Is.True);
        Assert.That(state.TryAddTileToSelectedSlot(102, out _), Is.True);

        var replaced = state.TryReplaceSlotWithTilesFromHand(0, new[] { 201, 201, 201 }, out var reason);

        Assert.That(replaced, Is.True, reason);
        CollectionAssert.AreEqual(new[] { 201, 201, 201 }, state.GetSlotTiles(0).ToArray());
        CollectionAssert.AreEqual(new[] { 101, 102, 103, 401 }, state.HandTiles.ToArray());
    }

    [Test]
    public void CleanupForNextTurn_ClearsHandMeldSlots_AndAdvancesTurn()
    {
        var state = new TurnState(new[] { 101, 102, 103, 201, 201, 201 });
        state.SetHeadTile(407);
        state.SelectSlot(1);

        Assert.That(state.TryAddTileToSelectedSlot(201, out _), Is.True);
        Assert.That(state.TryAddTileToSelectedSlot(201, out _), Is.True);
        state.CreateMeld(Project.Core.Melds.MeldType.Koutsu, new[] { 101, 101, 101 });

        state.CleanupForNextTurn();

        Assert.That(state.TurnIndex, Is.EqualTo(2));
        Assert.That(state.Phase, Is.EqualTo(TurnPhase.Draw));
        Assert.That(state.HeadTileId, Is.EqualTo(0));
        Assert.That(state.HandTiles.Count, Is.EqualTo(0));
        Assert.That(state.Melds.Count, Is.EqualTo(0));
        Assert.That(state.GetSlotTiles(0).Count, Is.EqualTo(0));
        Assert.That(state.GetSlotTiles(1).Count, Is.EqualTo(0));
        Assert.That(state.GetSlotTiles(2).Count, Is.EqualTo(0));
        Assert.That(state.GetSlotTiles(3).Count, Is.EqualTo(0));
    }
}
