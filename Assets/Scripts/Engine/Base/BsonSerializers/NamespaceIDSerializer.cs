using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace PVZEngine.BsonSerializers
{
    public class NamespaceIDSerializer : ClassSerializerBase<NamespaceID>
    {
        public NamespaceIDSerializer(string defaultNsp)
        {
            this.defaultNsp = defaultNsp;
        }
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NamespaceID value)
        {
            var writer = context.Writer;
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteString(value.ToString());
            }
        }

        public override NamespaceID Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            if (reader.GetCurrentBsonType() != BsonType.String)
            {
                reader.ReadNull();
                return null;
            }
            if (NamespaceID.TryParse(reader.ReadString(), defaultNsp, out var parsed))
            {
                return parsed;
            }
            return null;
        }
        private string defaultNsp;
    }
}
