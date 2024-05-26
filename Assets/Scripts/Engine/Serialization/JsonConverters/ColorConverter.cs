using System;
using Newtonsoft.Json;
using UnityEngine;

namespace PVZEngine.Serialization
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.r);
            writer.WriteValue(value.g);
            writer.WriteValue(value.b);
            writer.WriteValue(value.a);
            writer.WriteEndArray();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = serializer.Deserialize<float[]>(reader);
            return new Color(value[0], value[1], value[2], value[3]);
        }
    }
}
