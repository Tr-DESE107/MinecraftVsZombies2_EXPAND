using MongoDB.Bson.Serialization;

namespace Tools.BsonSerializers
{
    public class RandomGeneratorSerializer : ArraySerializerBase<RandomGenerator>
    {
        protected override void WriteArrayValues(BsonSerializationContext context, BsonSerializationArgs args, RandomGenerator value)
        {
            var writer = context.Writer;
            var serializable = value.ToSerializable();
            writer.WriteInt32(serializable.x);
            writer.WriteInt32(serializable.y);
            writer.WriteInt32(serializable.z);
            writer.WriteInt32(serializable.w);
        }

        protected override RandomGenerator ReadArrayValues(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            int w = reader.ReadInt32();
            return RandomGenerator.FromSerializable(new SerializableRNG(x, y, z, w));
        }
    }
}
