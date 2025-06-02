using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.HeldItems
{
    [HeldItemBehaviourDefinition(VanillaHeldItemBehaviourNames.pickup)]
    public class PickupHeldItemBehaviour : HeldItemBehaviourDefinition
    {
        public PickupHeldItemBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidFor(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return false;
            if (pointerParams.IsInvalidClickButton())
                return false;
            var entity = entityTarget.Target;
            var level = entity.Level;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (level.IsHoldingExclusiveItem() && entity.IsStrictCollect())
                    {
                        return false;
                    }
                    return !entity.IsCollected() && !level.IsHoldingEntity(entity);
            }
            return false;
        }
        public override void OnPointerEvent(IHeldItemTarget target, IHeldItemData data, PointerInteractionData pointerParams)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return;
            OnMainPointerEvent(entityTarget, data, pointerParams);
        }
        private void OnMainPointerEvent(HeldItemTargetEntity entityTarget, IHeldItemData data, PointerInteractionData pointerParams)
        {
            var interaction = pointerParams.interaction;
            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    bool interacted = interaction == PointerInteraction.Down || interaction == PointerInteraction.Hold || interaction == PointerInteraction.Streak;
                    if (entity.IsStrictCollect())
                    {
                        interacted = interaction == PointerInteraction.Down;
                    }
                    if (interacted)
                    {
                        if (entity.CanCollect())
                        {
                            entity.Collect();
                        }
                        else
                        {
                            if (!entity.Level.IsPlayingSound(VanillaSoundID.buzzer))
                            {
                                entity.PlaySound(VanillaSoundID.buzzer);
                            }
                        }
                    }
                    break;
            }
        }
    }
}
