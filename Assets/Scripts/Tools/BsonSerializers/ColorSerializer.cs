using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class ColorSerializer : StructSerializerBase<Color>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Color value)
        {
            var writer = context.Writer;
            var code = ColorUtility.ToHtmlStringRGBA(value);
            writer.WriteString($"#{code}");
        }

        public override Color Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            var code = reader.ReadString();
            if (ColorUtility.TryParseHtmlString(code, out var color))
            {
                return color;
            }
            return Color.clear;
        }
    }
}
