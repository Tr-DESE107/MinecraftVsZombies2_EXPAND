using System;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableBuff
    {
        public NamespaceID definitionID;
        public SerializablePropertyDictionary propertyDict;
    }
}
