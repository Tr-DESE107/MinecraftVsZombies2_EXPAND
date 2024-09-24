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
            writer.WriteString(value.ToString());
        }

        public override NamespaceID Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            return NamespaceID.Parse(reader.ReadString(), defaultNsp);
        }
        private string defaultNsp;
    }
}
