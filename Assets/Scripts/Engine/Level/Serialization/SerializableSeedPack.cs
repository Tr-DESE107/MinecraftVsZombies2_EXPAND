using System;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableSeedPack
    {
        public NamespaceID seedID;
        public SerializableBuffList buffs;
        public SerializablePropertyDictionary propertyDict;
    }
}
