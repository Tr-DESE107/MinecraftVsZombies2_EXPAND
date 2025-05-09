using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Tools
{
    public class Vector4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteValue(value.z);
            writer.WriteValue(value.w);
            writer.WriteEndArray();
        }

        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<float[]>(reader);
            return new Vector4(value[0], value[1], value[2], value[3]);
        }
    }
}
