using System;

namespace PVZEngine.Serialization
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
