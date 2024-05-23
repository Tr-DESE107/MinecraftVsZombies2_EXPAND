namespace PVZEngine
{
    public class GameOption
    {
        public int LeftFaction { get; set; }
        public int RightFaction { get; set; }
        public int TPS { get; set; } = 30;
        public int CardSlotCount { get; set; }
        public int StarshardSlotCount { get; set; }
        public float StartEnergy { get; set; }
        public float MaxEnergy { get; set; }
    }
}
