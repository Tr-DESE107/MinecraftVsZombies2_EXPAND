﻿using MongoDB.Bson.Serialization;
using UnityEngine;

namespace Tools.BsonSerializers
{
    public class Vector2IntSerializer : ArraySerializerBase<Vector2Int>
    {
        protected override void WriteArrayValues(BsonSerializationContext context, BsonSerializationArgs args, Vector2Int value)
        {
            var writer = context.Writer;
            writer.WriteInt32(value.x);
            writer.WriteInt32(value.y);
        }

        protected override Vector2Int ReadArrayValues(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var reader = context.Reader;
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            return new Vector2Int(x, y);
        }
    }
}
