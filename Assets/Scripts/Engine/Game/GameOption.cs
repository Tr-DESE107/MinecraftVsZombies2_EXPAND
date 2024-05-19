namespace PVZEngine
{
    public class GameOption
    {
        public int LeftFaction { get; private set; }
        public int RightFaction { get; private set; }
        public int TPS { get; private set; } = 30;
        public int CardSlotCount { get; private set; }
        public int StarshardSlotCount { get; private set; }
        public float StartEnergy { get; private set; }
    }
}
