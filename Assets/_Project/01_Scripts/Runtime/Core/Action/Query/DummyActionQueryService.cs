using System.Collections.Generic;
using Project.InputSystem;
using Project.Core.Turn;
using Project.UI.Models;
using Project.Core.Tiles;

namespace Project.Core.Action.Query
{
    public class DummyActionQueryService : IActionQueryService
    {
        public ActionMenuModel Query(ActionRequest request, TurnSnapshot snapshot)
        {
            var model = new ActionMenuModel();

            if (request.TargetType != ActionTargetType.Tile)
                return model;

            int tile = request.TargetId;

            // 1) 슌쯔 후보
            var shuntsu = GetPlayableShuntsu(tile, snapshot.HandTiles);
            foreach (var cand in shuntsu)
            {
                model.Add(
                    ActionCommandType.CreateShuntsu,
                    label: "Shuntsu",
                    previewTiles: new[] { cand.A, cand.B, cand.C },
                    payload: cand
                );
            }

            // 2) 컷쯔
            int count = CountOf(snapshot.HandTiles, tile);
            if (count >= 3)
            {
                model.Add(
                    ActionCommandType.CreateKoutsu,
                    label: "Koutsu",
                    previewTiles: new[] { tile, tile, tile },
                    payload: tile
                );
            }

            // 3) 깡(불가역)
            if (count >= 4)
            {
                model.Add(
                    ActionCommandType.PromoteToKan,
                    label: "Kan",
                    previewTiles: new[] { tile, tile, tile, tile },
                    isDangerous: true,
                    payload: tile
                );
            }

            return model;
        }

        private static List<MeldCandidate> GetPlayableShuntsu(int tile, IReadOnlyList<int> hand)
        {
            // Honors never make shuntsu.
            if ((tile / 100) == 4)
                return new List<MeldCandidate>();

            var shapes = MeldCandidateCalculator.GetShuntsuShapes(tile);
            var playable = new List<MeldCandidate>();

            foreach (var cand in shapes)
            {
                if (HasAtLeast(hand, cand.A, 1) &&
                    HasAtLeast(hand, cand.B, 1) &&
                    HasAtLeast(hand, cand.C, 1))
                {
                    playable.Add(cand);
                }
            }

            return playable;
        }

        private static bool HasAtLeast(IReadOnlyList<int> hand, int tile, int required)
            => CountOf(hand, tile) >= required;

        private static int CountOf(IReadOnlyList<int> hand, int tile)
        {
            int c = 0;
            for (int i = 0; i < hand.Count; i++)
                if (hand[i] == tile) c++;
            return c;
        }
    }
}
