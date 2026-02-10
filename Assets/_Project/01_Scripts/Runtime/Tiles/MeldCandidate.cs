namespace Project.Core.Tiles
{
    public readonly struct MeldCandidate
    {
        public readonly int A;
        public readonly int B;
        public readonly int C;

        public MeldCandidate(int a, int b, int c)
        {
            A = a; B = b; C = c;
        }

        public override string ToString() => $"{A},{B},{C}";
    }
}