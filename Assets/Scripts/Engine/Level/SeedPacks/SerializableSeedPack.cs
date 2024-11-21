using System;
using PVZEngine.Buffs;

namespace PVZEngine.SeedPacks
{
    [Serializable]
    public class SerializableSeedPack
    {
        public int id;
        public NamespaceID seedID;
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyDictionary propertyDict;
    }
}
