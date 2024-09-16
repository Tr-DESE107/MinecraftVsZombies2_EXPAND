using System.Collections.Generic;
using PVZEngine.Level;
using Tools;

namespace PVZEngine.Serialization
{
    public class SerializableLevel
    {
        private Entity killerEnemy;
        private string deathMessage;

        private bool isRerun;

        private float gridSize;
        private float gridLeftX;
        private float gridBottomZ;
        private int maxLaneCount;
        private int maxColumnCount;


        public int seed;
        public bool isCleared;
        public NamespaceID stageDefinitionID;
        public NamespaceID areaDefinitionID;
        public bool isEndless;
        public NamespaceID difficulty;
        public SerializableLevelOption Option;
        public SerializableRNG levelRandom;
        public SerializableRNG entityRandom;
        public SerializableRNG effectRandom;
        public SerializableRNG roundRandom;
        public SerializableRNG spawnRandom;
        public SerializableRNG conveyorRandom;
        public SerializableRNG debugRandom;
        public SerializableRNG miscRandom;
        public SerializablePropertyDictionary propertyDict;
        public SerializableGrid[] grids;
        public float rechargeSpeed;
        public float rechargeTimeMultiplier;
        public SerializableSeedPack[] seedPacks;
        public bool requireCards;
        public int currentEntityID = 1;
        public List<SerializableEntity> entities;
        public float energy;
        public Dictionary<int, float> delayedEnergyEntities;
        public int currentWave;
        public int currentFlag;
        public int waveState;
        public bool levelProgressVisible;
        public List<int> spawnedLanes;
        public List<NamespaceID> spawnedID;

        public SerializableBuffList buffs;

        public Dictionary<NamespaceID, ISerializableLevelComponent> components;
    }
}
