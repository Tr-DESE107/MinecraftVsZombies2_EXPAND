using System;
using PVZEngine.Buffs;

namespace PVZEngine.SeedPacks
{
    public abstract class SerializableSeedPack
    {
        public long id;
        public NamespaceID seedID;
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyDictionary propertyDict;
    }
    [Serializable]
    public class SerializableClassicSeedPack : SerializableSeedPack
    {
    }
    [Serializable]
    public class SerializableConveyorSeedPack : SerializableSeedPack
    {
    }
}
