using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using PVZEngine;
using Tools.BsonSerializers;

namespace MVZ2Logic.Level
{
    public class EndlessSpawnEntry : IEnemySpawnEntry
    {
        public EndlessSpawnEntry(NamespaceID spawnRef)
        {
            SpawnRef = spawnRef;
        }
        public NamespaceID SpawnRef { get; }

        public int EarliestFlag => 0;
    }
    public class EndlessSpawnEntrySerializer : WrappedSerializerBase<EndlessSpawnEntry>
    {
        public EndlessSpawnEntrySerializer()
        {
        }
        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, EndlessSpawnEntry value)
        {
            var writer = context.Writer;
            writer.WriteStartDocument();
            writer.WriteName("spawnRef");
            var serializer = BsonSerializer.LookupSerializer<NamespaceID>();
            serializer.Serialize(context, args, value.SpawnRef);
            writer.WriteEndDocument();
        }

        protected override EndlessSpawnEntry DeserializeClassValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;

            var bsonType = reader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Document:
                    reader.ReadStartDocument();
                    reader.ReadName("spawnRef");
                    var serializer = BsonSerializer.LookupSerializer<NamespaceID>();
                    var spawnRef = serializer.Deserialize(context, args);
                    reader.ReadEndDocument();
                    return new EndlessSpawnEntry(spawnRef);

                default:
                    throw CreateCannotDeserializeFromBsonTypeException(bsonType);
            }
        }
    }
}
