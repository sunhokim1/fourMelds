namespace Project.InputSystem
{
    public interface IActionRequestSink
    {
        void Handle(ActionRequest request);
    }
}