using MongoDB.Bson.Serialization.Attributes;

namespace MVZ2.Level
{
    [BsonIgnoreExtraElements]
    public class SerializableLevelControllerHeader
    {
        public LevelDataIdentifierList identifiers;
    }
}
