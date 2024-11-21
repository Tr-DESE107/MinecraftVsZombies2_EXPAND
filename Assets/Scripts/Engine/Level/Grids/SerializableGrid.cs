using System;
using System.Collections.Generic;

namespace PVZEngine.Grids
{
    [Serializable]
    public class SerializableGrid
    {
        public int lane;
        public int column;
        public NamespaceID definitionID;
        public List<long> takenEntities;
    }
}
