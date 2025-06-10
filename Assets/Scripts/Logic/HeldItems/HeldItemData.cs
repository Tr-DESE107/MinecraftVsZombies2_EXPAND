using MVZ2.HeldItems;
using PVZEngine;

namespace MVZ2Logic.HeldItems
{
    public struct HeldItemData : IHeldItemData
    {
        public NamespaceID Type { get; set; }
        public long ID { get; set; }
        public HeldItemDefinition Definition { get; set; }
        public int Priority { get; set; }
        public bool NoCancel { get; set; }
        public bool InstantTrigger { get; set; }
        public bool InstantEvoke { get; set; }
    }
}
