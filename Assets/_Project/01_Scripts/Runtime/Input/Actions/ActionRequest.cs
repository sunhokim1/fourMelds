namespace Project.InputSystem
{
    public readonly struct ActionRequest
    {
        public readonly ActionRequestType RequestType;
        public readonly ActionTargetType TargetType;
        public readonly int TargetId;

        public ActionRequest(
            ActionRequestType requestType,
            ActionTargetType targetType,
            int targetId)
        {
            RequestType = requestType;
            TargetType = targetType;
            TargetId = targetId;
        }

        public override string ToString()
            => $"{RequestType} ({TargetType}:{TargetId})";
    }
}