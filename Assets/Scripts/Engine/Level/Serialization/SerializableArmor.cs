using System.Collections.Generic;

namespace PVZEngine.Serialization
{
    public class SerializableArmor
    {
        public NamespaceID definitionID;
        public float health;
        public List<SerializableBuff> buffs;
        public SerializablePropertyDictionary propertyDict;
    }
}
