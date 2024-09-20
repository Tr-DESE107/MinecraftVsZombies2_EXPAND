using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class Vector3Serializer : StructSerializerBase<Vector3>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Vector3 value)
        {
            var writer = context.Writer;
            writer.WriteStartArray();
            writer.WriteDouble(value.x);
            writer.WriteDouble(value.y);
            writer.WriteDouble(value.z);
            writer.WriteEndArray();
        }

        public override Vector3 Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartArray();
            float x = (float)reader.ReadDouble();
            float y = (float)reader.ReadDouble();
            float z = (float)reader.ReadDouble();
            reader.ReadEndArray();
            return new Vector3(x, y, z);
        }
    }
}
