using PVZEngine;
using UnityEngine;

namespace MVZ2Logic.Map
{
    public class MapMeta
    {
        public string id;
        public NamespaceID area;
        public NamespaceID endlessUnlock;
        public MapPreset[] presets;
        public NamespaceID[] stages;
    }
    public class MapPreset
    {
        public NamespaceID id;
        public NamespaceID model;
        public NamespaceID music;
        public Color backgroundColor;
    }
}
