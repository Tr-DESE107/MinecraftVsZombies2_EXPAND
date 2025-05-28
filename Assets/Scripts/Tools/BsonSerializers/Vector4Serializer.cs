﻿using MongoDB.Bson.Serialization;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class Vector4Serializer : ArraySerializerBase<Vector4>
    {
        protected override void WriteArrayValues(BsonSerializationContext context, BsonSerializationArgs args, Vector4 value)
        {
            var writer = context.Writer;
            writer.WriteDouble(value.x);
            writer.WriteDouble(value.y);
            writer.WriteDouble(value.z);
            writer.WriteDouble(value.w);
        }

        protected override Vector4 ReadArrayValues(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            float x = (float)reader.ReadDouble();
            float y = (float)reader.ReadDouble();
            float z = (float)reader.ReadDouble();
            float w = (float)reader.ReadDouble();
            return new Vector4(x, y, z, w);
        }
    }
}
