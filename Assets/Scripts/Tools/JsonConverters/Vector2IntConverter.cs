using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Tools
{
    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteEndArray();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<int[]>(reader);
            return new Vector2Int(value[0], value[1]);
        }
    }
}
