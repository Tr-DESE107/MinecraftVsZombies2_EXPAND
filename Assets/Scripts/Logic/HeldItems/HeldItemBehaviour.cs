using MVZ2.HeldItems;
using MVZ2Logic;
using MVZ2Logic.HeldItems;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemBehaviour
    {
        public HeldItemBehaviour(HeldItemDefinition definition)
        {
            Definition = definition;
        }

        public virtual bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            return false;
        }
        public virtual HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            return HeldHighlight.None;
        }
        public virtual void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
        }
        public HeldItemDefinition Definition { get; }
    }
}
