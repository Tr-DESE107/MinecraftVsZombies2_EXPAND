using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.HeldItems
{
    public class PickupHeldItemBehaviour : HeldItemBehaviour
    {
        public PickupHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        public override bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return false;
            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    return !entity.IsCollected();
            }
            return false;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return;
            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (!entity.IsCollected())
                    {
                        bool interacted = interaction != PointerInteraction.Release;
                        if (entity.IsStrictCollect())
                        {
                            interacted = interaction == PointerInteraction.Press;
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
                    }
                    break;
            }
        }
    }
}
