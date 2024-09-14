using PVZEngine.Serialization;

namespace PVZEngine.Level
{
    public class LevelOption
    {
        public int LeftFaction { get; set; }
        public int RightFaction { get; set; }
        public int TPS { get; set; } = 30;
        public int CardSlotCount { get; set; }
        public int StarshardSlotCount { get; set; }
        public float StartEnergy { get; set; }
        public float MaxEnergy { get; set; }
        public SerializableLevelOption Serialize()
        {
            return new SerializableLevelOption()
            {
                leftFaction = LeftFaction,
                rightFaction = RightFaction,
                tps = TPS,
                cardSlotCount = CardSlotCount,
                starshardSlotCount = StarshardSlotCount,
                startEnergy = StartEnergy,
                maxEnergy = MaxEnergy,
            };
        }
        public static LevelOption Deserialize(SerializableLevelOption seri)
        {
            return new LevelOption()
            {
                LeftFaction = seri.leftFaction,
                RightFaction = seri.rightFaction,
                TPS = seri.tps,
                CardSlotCount = seri.cardSlotCount,
                StarshardSlotCount = seri.starshardSlotCount,
                StartEnergy = seri.startEnergy,
                MaxEnergy = seri.maxEnergy
            };
        }
    }
}
