using System;
using PVZEngine.Auras;

namespace PVZEngine.Buffs
{
    [Serializable]
    public class SerializableBuff
    {
        public long id;
        public NamespaceID definitionID;
        public SerializablePropertyDictionary propertyDict;
        public SerializableAuraEffect[] auras;
    }
}
