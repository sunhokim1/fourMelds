using Project.Core.Turn;
using Project.UI.Models;

namespace Project.Core.Action.Execute
{
    public interface IActionExecutionService
    {
        bool Execute(ActionCommand command, TurnState state, out string failReason);
    }
}