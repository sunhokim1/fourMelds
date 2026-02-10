namespace FourMelds.Combat
{
    /// <summary>
    /// C# 9.0 호환: record struct 대신 readonly struct 사용
    /// </summary>
    public readonly struct TurnIndex
    {
        public int Value { get; }

        public TurnIndex(int value)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();
    }
}
