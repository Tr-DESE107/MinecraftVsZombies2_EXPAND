using System;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableArmor
    {
        public NamespaceID definitionID;
        public float health;
        public long currentBuffID;
        public SerializableBuffList buffs;
        public SerializablePropertyDictionary propertyDict;
    }
}
