using System;
using PVZEngine.Auras;
using PVZEngine.Level;

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
