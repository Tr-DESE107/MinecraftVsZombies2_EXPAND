using PVZEngine.Serialization;

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
        public SerializableGameOption Serialize()
        {
            return new SerializableGameOption()
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
        public static GameOption Deserialize(SerializableGameOption seri)
        {
            return new GameOption()
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
