using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Level.Collisions;

namespace MVZ2Logic.Entities
{
    public interface IArmorSlotMeta
    {
        string Name { get; }
        string Anchor { get; }
    }
    public interface IArmorMeta
    {
        string ID { get; }
        NamespaceID[] Behaviours { get; }
        ColliderConstructor[] ColliderConstructors { get; }
        Dictionary<string, object> Properties { get; }
    }
}
