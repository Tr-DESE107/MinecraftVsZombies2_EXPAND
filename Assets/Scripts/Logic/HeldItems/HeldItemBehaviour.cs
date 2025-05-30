using MVZ2.HeldItems;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2Logic.HeldItems
{
    public abstract class HeldItemBehaviour
    {
        public HeldItemBehaviour(HeldItemDefinition definition)
        {
            Definition = definition;
        }
        public virtual bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return false;
        }
        public virtual HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            return HeldHighlight.None;
        }
        public virtual void OnBegin(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnEnd(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnUpdate(LevelEngine level, IHeldItemData data)
        {
        }
        public virtual void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
        }
        public virtual void OnSetModel(LevelEngine level, IHeldItemData data, IModelInterface model)
        {
        }
        public HeldItemDefinition Definition { get; }
    }
}
