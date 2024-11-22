using System.Collections.Generic;
using PVZEngine;

namespace MVZ2Logic.Entities
{
    public class EntityMeta
    {
        public int type;
        public string id;
        public string name;
        public string deathMessage;
        public string tooltip;
        public NamespaceID unlock;
        public int order;
        public Dictionary<string, object> properties;
    }
}
