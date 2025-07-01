using System;
using System.Collections.Generic;
using PVZEngine.Level;

namespace PVZEngine.Grids
{
    [Serializable]
    public class SerializableGrid
    {
        public int lane;
        public int column;
        public NamespaceID definitionID;
        public Dictionary<string, long> layerEntities;
        public SerializablePropertyBlock properties;
    }
}
