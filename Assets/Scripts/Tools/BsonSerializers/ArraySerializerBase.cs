using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Tools.BsonSerializers
{
    public abstract class ArraySerializerBase<TValue> : SerializerBase<TValue>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TValue value)
        {
            var writer = context.Writer;
            writer.WriteStartArray();
            WriteArrayValues(context, args, value);
            writer.WriteEndArray();
        }

        public override TValue Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;

            var bsonType = reader.GetCurrentBsonType();
            switch (bsonType)
            {
                case BsonType.Null:
                    reader.ReadNull();
                    return default;

                case BsonType.Array:
                    reader.ReadStartArray();
                    TValue value = ReadArrayValues(context, args);
                    reader.ReadEndArray();
                    return value;

                case BsonType.Document:
                    var serializer = new DiscriminatedWrapperSerializer<TValue>(_discriminatorConvention, this);
                    if (serializer.IsPositionedAtDiscriminatedWrapper(context))
                    {
                        return serializer.Deserialize(context);
                    }
                    else
                    {
                        goto default;
                    }

                default:
                    throw CreateCannotDeserializeFromBsonTypeException(bsonType);
            }
        }
        protected abstract void WriteArrayValues(BsonSerializationContext context, BsonSerializationArgs args, TValue value);
        protected abstract TValue ReadArrayValues(BsonDeserializationContext context, BsonDeserializationArgs args);
        private readonly IDiscriminatorConvention _discriminatorConvention = new ScalarDiscriminatorConvention("_t");
    }
}
