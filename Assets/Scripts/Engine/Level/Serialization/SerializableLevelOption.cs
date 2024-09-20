using System;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableLevelOption
    {
        public int leftFaction;
        public int rightFaction;
        public int tps;
        public int cardSlotCount;
        public int starshardSlotCount;
        public float startEnergy;
        public float maxEnergy;
    }
}
