using System.Collections.Generic;

namespace Project.UI.Models
{
    public class ActionMenuModel
    {
        public List<ActionOption> Options { get; } = new();

        public void Add(ActionOption option) => Options.Add(option);

        public void Add(
            ActionCommandType command,
            string label,
            int[] previewTiles,
            bool isDangerous = false,
            object payload = null)
        {
            Options.Add(new ActionOption(command, label, previewTiles, isDangerous, payload));
        }
    }
}