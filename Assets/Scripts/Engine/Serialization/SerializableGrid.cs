using System.Collections.Generic;

namespace PVZEngine.Serialization
{
    public class SerializableGrid
    {
        public int lane;
        public int column;
        public NamespaceID definitionID;
        public List<int> takenEntities;
    }
}
