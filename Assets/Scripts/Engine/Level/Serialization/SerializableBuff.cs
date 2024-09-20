using System;
using PVZEngine.Level;

namespace PVZEngine.Serialization
{
    [Serializable]
    public class SerializableBuff
    {
        public NamespaceID definitionID;
        public ISerializeBuffTarget target;
        public SerializablePropertyDictionary propertyDict;
    }
}
