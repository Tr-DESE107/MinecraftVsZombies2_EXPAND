using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.triggerCart)]
    public class TriggerCartHeldItemBehaviour : HeldItemBehaviourDefinition
    {
        public TriggerCartHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override HeldTargetFlag GetHeldTargetMask(LevelEngine level)
        {
            return HeldTargetFlag.Cart;
        }
        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return false;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.CART:
                    return !entity.IsCartTriggered();
            }
            return false;
        }
        public override HeldHighlight GetHighlight(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointer)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return HeldHighlight.None;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.CART:
                    return HeldHighlight.Entity(entity);
            }
            return HeldHighlight.None;
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return;
            if (pointerParams.IsInvalidClickButton())
                return;
            var interaction = pointerParams.interaction;
            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.CART:
                    if (interaction == PointerInteraction.Hold)
                    {
                        if (!entity.IsCartTriggered())
                        {
                            entity.ChargeUpCartTrigger();
                        }
                    }
                    break;
            }
        }
    }
}
