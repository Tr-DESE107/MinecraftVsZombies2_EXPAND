using System.Collections.Generic;
using UnityEngine;

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
