using System;
using PVZEngine.Level;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableBuff
    {
        public NamespaceID definitionID;
        public SerializablePropertyDictionary propertyDict;
    }
}
