using Project.Core.Turn;
using Project.UI.Models;
using Project.Core.Tiles; // MeldCandidate
using FourMelds.Cards;    // ✅ CardRegistry, CardContext

namespace Project.Core.Action.Execute
{
    public class DummyActionExecutionService : IActionExecutionService
    {
        public bool Execute(ActionCommand command, TurnState state, out string failReason)
        {
            failReason = null;

            switch (command.Type)
            {
                case ActionCommandType.PlayCard:
                    {
                        if (command.Payload is not int cardIndex)
                        {
                            failReason = "Invalid payload for PlayCard";
                            return false;
                        }

                        return CardPlayService.TryPlay(cardIndex, state, out failReason);
                    }

                case ActionCommandType.CreateShuntsu:
                    {
                        // Payload는 MeldCandidate (A,B,C)
                        if (command.Payload is not MeldCandidate cand)
                        {
                            failReason = "Invalid payload for Shuntsu";
                            return false;
                        }

                        if (!IsValidShuntsu(cand))
                        {
                            failReason = "Invalid shuntsu shape";
                            return false;
                        }

                        if (!HasAtLeast(state, cand.A, 1) ||
                            !HasAtLeast(state, cand.B, 1) ||
                            !HasAtLeast(state, cand.C, 1))
                        {
                            failReason = "Tiles missing in hand";
                            return false;
                        }

                        state.TryRemoveTile(cand.A);
                        state.TryRemoveTile(cand.B);
                        state.TryRemoveTile(cand.C);

                        state.CreateMeld(Project.Core.Melds.MeldType.Shuntsu, new[] { cand.A, cand.B, cand.C });
                        return true;
                    }

                case ActionCommandType.CreateKoutsu:
                    {
                        // Payload는 tileId(int)
                        if (command.Payload is not int tile)
                        {
                            failReason = "Invalid payload for Koutsu";
                            return false;
                        }

                        if (state.CountOf(tile) < 3)
                        {
                            failReason = "Not enough tiles for Koutsu";
                            return false;
                        }

                        // 3장 제거
                        state.TryRemoveTile(tile);
                        state.TryRemoveTile(tile);
                        state.TryRemoveTile(tile);

                        state.CreateMeld(Project.Core.Melds.MeldType.Koutsu, new[] { tile, tile, tile });
                        return true;
                    }

                case ActionCommandType.PromoteToKan:
                    {
                        if (command.Payload is not int kanTileId)
                        {
                            failReason = "Invalid payload for Kan";
                            return false;
                        }

                        int slot = state.SelectedSlotIndex;
                        return state.TryPromoteToKanInSlot(slot, kanTileId, out failReason);
                    }

                default:
                    failReason = "Not implemented";
                    return false;
            }
        }

        private static bool HasAtLeast(TurnState state, int tileId, int count)
            => state.CountOf(tileId) >= count;

        private static bool IsValidShuntsu(MeldCandidate cand)
        {
            int suitGroup = cand.A / 100;
            if (suitGroup < 1 || suitGroup > 3)
                return false;

            var a = new TileId(cand.A);
            var b = new TileId(cand.B);
            var c = new TileId(cand.C);

            if (!a.IsNumberSuit || !b.IsNumberSuit || !c.IsNumberSuit)
                return false;

            if (a.Suit != b.Suit || b.Suit != c.Suit)
                return false;

            int ra = a.Rank;
            int rb = b.Rank;
            int rc = c.Rank;
            return rb == ra + 1 && rc == rb + 1;
        }
    }
}
