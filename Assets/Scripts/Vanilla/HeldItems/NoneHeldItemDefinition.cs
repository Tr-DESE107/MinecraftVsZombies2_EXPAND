using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Entities;

namespace MVZ2.Vanilla.HeldItems
{
    [HeldItemDefinition(BuiltinHeldItemNames.none)]
    public class NoneHeldItemDefinition : HeldItemDefinition
    {
        public NoneHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        #region 实体
        public override bool CheckRaycast(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return false;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    return !entity.IsCollected();
                case EntityTypes.CART:
                    return !entity.IsCartTriggered();
            }
            return false;
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return HeldHighlight.None;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.CART:
                    return HeldHighlight.Entity;
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            if (target is not HeldItemTargetEntity entityTarget)
                return;

            var entity = entityTarget.Target;
            switch (entity.Type)
            {
                case EntityTypes.PICKUP:
                    if (interaction != PointerInteraction.Release)
                    {
                        if (!entity.IsCollected())
                            entity.Collect();
                    }
                    break;
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
        #endregion
    }
}
