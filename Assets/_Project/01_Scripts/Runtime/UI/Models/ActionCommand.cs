namespace Project.UI.Models
{
    public readonly struct ActionCommand
    {
        public ActionCommandType Type { get; }
        public int TargetId { get; }
        public object Payload { get; }

        public ActionCommand(ActionCommandType type, int targetId, object payload)
        {
            Type = type;
            TargetId = targetId;
            Payload = payload;
        }

        public override string ToString()
            => $"{Type} target={TargetId} payload={Payload}";
    }
}