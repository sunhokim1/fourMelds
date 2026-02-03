using System;

namespace Project.InputSystem
{
    public interface IActionRequestSource
    {
        event Action<ActionRequest> OnRequest;
    }
}