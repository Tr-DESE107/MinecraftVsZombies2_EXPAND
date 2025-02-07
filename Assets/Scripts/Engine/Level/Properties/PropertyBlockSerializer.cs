using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Tools.BsonSerializers;

namespace PVZEngine.Level.BsonSerializers
{
    public class PropertyBlockSerializer : WrappedSerializerBase<SerializablePropertyBlock>
    {
        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, SerializablePropertyBlock value)
        {
            var writer = context.Writer;
            writer.WriteStartDocument();
            foreach (var pair in value.modifiable.properties.properties)
            {
                var key = pair.Key;
                writer.WriteName(key);
                BsonSerializer.Serialize(writer, typeof(object), pair.Value);
            }
            writer.WriteEndDocument();
        }
        protected override SerializablePropertyBlock DeserializeClassValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var properties = new Dictionary<string, object>();

            var reader = context.Reader;

            var bsonType = reader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Document:
                    reader.ReadStartDocument();
                    while (reader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        var key = reader.ReadName();
                        var value = BsonSerializer.Deserialize(reader, typeof(object));
                        properties.Add(key, value);
                    }
                    reader.ReadEndDocument();
                    return new SerializablePropertyBlock()
                    {
                        modifiable = new SerializableModifiableProperties()
                        {
                            properties = new SerializablePropertyDictionary()
                            {
                                properties = properties
                            }
                        },
                    };

                default:
                    throw CreateCannotDeserializeFromBsonTypeException(bsonType);
            }
        }
        private void DeserializeDictionary(IBsonReader reader, Dictionary<string, object> properties)
        {
            reader.ReadStartDocument();
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var key = reader.ReadName();
                var value = BsonSerializer.Deserialize(reader, typeof(object));
                properties.Add(key, value);
            }
            reader.ReadEndDocument();
        }
    }
}
