namespace Project.Core.Melds
{
    public class MeldState
    {
        public int MeldId { get; }
        public MeldType Type { get; }
        public int[] Tiles { get; }
        public bool IsFixed { get; private set; }

        public MeldState(int meldId, MeldType type, int[] tiles, bool isFixed = false)
        {
            MeldId = meldId;
            Type = type;
            Tiles = tiles;
            IsFixed = isFixed;
        }

        public void Fix()
        {
            IsFixed = true;
        }
    }
}