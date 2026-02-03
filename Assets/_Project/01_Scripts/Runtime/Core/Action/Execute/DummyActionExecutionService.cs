using Project.Core.Turn;
using Project.UI.Models;
using Project.Core.Tiles; // MeldCandidate 쓰면 필요

namespace Project.Core.Action.Execute
{
    public class DummyActionExecutionService : IActionExecutionService
    {
        public bool Execute(ActionCommand command, TurnState state, out string failReason)
        {
            failReason = null;

            switch (command.Type)
            {
                case ActionCommandType.CreateShuntsu:
                    // Payload는 MeldCandidate (A,B,C)
                    if (command.Payload is not MeldCandidate cand)
                    {
                        failReason = "Invalid payload for Shuntsu";
                        return false;
                    }

                    if (!state.TryRemoveTile(cand.A) ||
                        !state.TryRemoveTile(cand.B) ||
                        !state.TryRemoveTile(cand.C))
                    {
                        failReason = "Tiles missing in hand";
                        return false;
                    }
                    state.CreateMeld(Project.Core.Melds.MeldType.Shuntsu, new[] { cand.A, cand.B, cand.C });
                    return true;

                case ActionCommandType.CreateKoutsu:
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

                default:
                    failReason = "Not implemented";
                    return false;
            }
        }
    }
}