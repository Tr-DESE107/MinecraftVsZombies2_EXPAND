using MVZ2Logic.HeldItems;
using PVZEngine;

namespace MVZ2.HeldItems
{
    public interface IHeldItemData
    {
        NamespaceID Type { get; }
        long ID { get; }
        HeldItemDefinition Definition { get; }
        int Priority { get; }
        bool NoCancel { get; }
        bool InstantTrigger { get; }
        bool InstantEvoke { get; }
    }
}
