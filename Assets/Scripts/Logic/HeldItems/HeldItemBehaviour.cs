using MVZ2.HeldItems;
using PVZEngine.Level;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemBehaviour
    {
        public HeldItemBehaviour(HeldItemDefinition definition)
        {
            Definition = definition;
        }
        public virtual void Update(LevelEngine level, IHeldItemData data)
        {
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
