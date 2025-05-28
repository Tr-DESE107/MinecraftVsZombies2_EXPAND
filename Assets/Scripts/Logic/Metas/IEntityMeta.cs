using System.Collections.Generic;
using PVZEngine;

namespace MVZ2Logic.Entities
{
    public interface IEntityMeta
    {
        int Type { get; }
        string ID { get; }
        string Name { get; }
        string DeathMessage { get; }
        string Tooltip { get; }
        NamespaceID Unlock { get; }
        int Order { get; }
        NamespaceID[] Behaviours { get; }
        Dictionary<string, object> Properties { get; }
    }
}
