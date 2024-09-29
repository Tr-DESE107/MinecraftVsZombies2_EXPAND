using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;

namespace MVZ2.Save
{
    public class MapPresetConfig
    {
        public MapPresetConfig(string id)
        {
            ID = id;
        }
        public SerializableMapPresetConfig ToSerializable()
        {
            return new SerializableMapPresetConfig()
            {
                id = ID,
                preset = Preset
            };
        }
        public static MapPresetConfig FromSerializable(SerializableMapPresetConfig serializable)
        {
            return new MapPresetConfig(serializable.id)
            {
                Preset = serializable.preset
            };
        }
        public string ID { get; private set; }
        public NamespaceID Preset { get; set; }
    }
    [Serializable]
    public class SerializableMapPresetConfig
    {
        public string id;
        public NamespaceID preset;
    }
}
