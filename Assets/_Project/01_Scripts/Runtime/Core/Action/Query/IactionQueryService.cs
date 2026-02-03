using Project.InputSystem;
using Project.Core.Turn;
using Project.UI.Models;

namespace Project.Core.Action.Query
{
    public interface IActionQueryService
    {
        ActionMenuModel Query(ActionRequest request, TurnSnapshot snapshot);
    }
}