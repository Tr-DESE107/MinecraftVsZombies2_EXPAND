using System;
using PVZEngine.Buffs;

namespace PVZEngine.Armors
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
