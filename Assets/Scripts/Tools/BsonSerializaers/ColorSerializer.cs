using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class ColorSerializer : StructSerializerBase<Color>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Color value)
        {
            var color32 = (Color32)value;
            var writer = context.Writer;
            writer.WriteStartArray();
            writer.WriteInt32(color32.r);
            writer.WriteInt32(color32.g);
            writer.WriteInt32(color32.b);
            writer.WriteInt32(color32.a);
            writer.WriteEndArray();
        }

        public override Color Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            reader.ReadStartArray();
            byte r = (byte)reader.ReadInt32();
            byte g = (byte)reader.ReadInt32();
            byte b = (byte)reader.ReadInt32();
            byte a = (byte)reader.ReadInt32();
            reader.ReadEndArray();
            return new Color32(r, g, b, a);
        }
    }
}
