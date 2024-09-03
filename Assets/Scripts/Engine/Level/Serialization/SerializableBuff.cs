using PVZEngine.LevelManaging;

namespace PVZEngine.Serialization
{
    public class SerializableBuff
    {
        public NamespaceID definitionID;
        public ISerializeBuffTarget target;
        public SerializablePropertyDictionary propertyDict = new SerializablePropertyDictionary();
    }
}
