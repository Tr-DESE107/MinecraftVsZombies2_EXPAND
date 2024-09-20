using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class Vector2Serializer : StructSerializerBase<Vector2>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector2 value)
        {
            var writer = context.Writer;
            writer.WriteStartArray();
            writer.WriteDouble(value.x);
            writer.WriteDouble(value.y);
            writer.WriteEndArray();
        }

        public override Vector2 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartArray();
            float x = (float)reader.ReadDouble();
            float y = (float)reader.ReadDouble();
            reader.ReadEndArray();
            return new Vector2(x, y);
        }
    }
}
