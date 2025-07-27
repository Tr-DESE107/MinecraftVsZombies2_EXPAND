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
        [Obsolete]
        public Dictionary<string, long> layerEntities;
        public Dictionary<string, long[]> layerEntityLists;
        public SerializablePropertyBlock properties;
    }
}
