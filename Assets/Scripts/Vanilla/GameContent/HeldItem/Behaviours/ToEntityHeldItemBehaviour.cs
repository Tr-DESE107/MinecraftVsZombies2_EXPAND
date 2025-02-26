using MVZ2.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine.Entities;

namespace MVZ2.GameContent.HeldItem
{
    public abstract class ToEntityHeldItemBehaviour : HeldItemBehaviour
    {
        public ToEntityHeldItemBehaviour(HeldItemDefinition definition) : base(definition)
        {
        }

        #region 实体
        public override bool IsValidFor(HeldItemTarget target, IHeldItemData data)
        {
            switch (target)
            {
                case HeldItemTargetEntity entityTarget:
                    return !Global.IsMobile() && CanUseOnEntity(entityTarget.Target);
                case HeldItemTargetGrid entityGrid:
                    return Global.IsMobile();
            }
            return false;
        }
        public override HeldHighlight GetHighlight(HeldItemTarget target, IHeldItemData data)
        {
            switch (target)
            {
                case HeldItemTargetEntity entityTarget:
                    {
                        var entity = entityTarget.Target;

                        var protector = entity;
                        var protectTarget = entity.GetProtectingTarget();
                        if (protectTarget == null)
                        {
                            return HeldHighlight.Entity;
                        }
                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnMain = CanUseOnEntity(protectTarget);
                        if (canUseOnProtector && canUseOnMain)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (entityTarget.PointerPosition.y < 0.5f)
                            {
                                return HeldHighlight.Entity;
                            }
                            else
                            {
                                return HeldHighlight.ProtectedEntity;
                            }
                        }
                        else if (canUseOnProtector)
                        {
                            // 只有保护层可以用。
                            return HeldHighlight.Entity;
                        }
                        else if (canUseOnMain)
                        {
                            // 只有主要层器械可以用。
                            return HeldHighlight.ProtectedEntity;
                        }
                        return HeldHighlight.None;
                    }
                case HeldItemTargetGrid gridTarget:
                    {
                        var grid = gridTarget.Target;

                        var protector = grid.GetProtectorEntity();
                        var main = grid.GetMainEntity();
                        var carrier = grid.GetCarrierEntity();

                        var protectedEntity = main;
                        if (!CanUseOnEntity(main))
                        {
                            protectedEntity = carrier;
                        }

                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnProtected = CanUseOnEntity(protectedEntity);
                        if (canUseOnProtector && canUseOnProtected)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (gridTarget.PointerPosition.y < 0.5f)
                            {
                                return HeldHighlight.LowerGreen;
                            }
                            else
                            {
                                return HeldHighlight.UpperGreen;
                            }
                        }
                        else if (canUseOnProtector)
                        {
                            // 只有保护层可以用。
                            if (main == null)
                            {
                                return HeldHighlight.Green;
                            }
                            else
                            {
                                return HeldHighlight.LowerGreen;
                            }
                        }
                        else if (canUseOnProtected)
                        {
                            // 只有主要层器械可以用。
                            if (protector == null)
                            {
                                return HeldHighlight.Green;
                            }
                            else
                            {
                                return HeldHighlight.UpperGreen;
                            }
                        }
                        return HeldHighlight.Red;
                    }
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerInteraction interaction)
        {
            switch (target)
            {
                case HeldItemTargetLawn lawnTarget:
                    {
                        var targetPhase = Global.IsMobile() ? PointerInteraction.Release : PointerInteraction.Press;
                        if (interaction != targetPhase)
                            return;

                        var level = lawnTarget.Level;
                        var area = lawnTarget.Area;
                        if (level.CancelHeldItem() && area == LawnArea.Side)
                        {
                            level.PlaySound(VanillaSoundID.tap);
                        }
                        return;
                    }
                case HeldItemTargetEntity entityTarget:
                    {
                        if (interaction != PointerInteraction.Press)
                            return;

                        var entity = entityTarget.Target;
                        var targetEntity = entity;
                        var protector = entity;
                        var protectTarget = entity.GetProtectingTarget();
                        if (protectTarget != null)
                        {
                            var canUseOnProtector = CanUseOnEntity(protector);
                            var canUseOnMain = CanUseOnEntity(protectTarget);
                            if (canUseOnProtector && canUseOnMain)
                            {
                                // 保护层和主要层器械都可以用，根据光标位置决定。
                                if (entityTarget.PointerPosition.y >= 0.5f)
                                {
                                    targetEntity = protectTarget;
                                }
                            }
                            else if (!canUseOnProtector)
                            {
                                // 只有主要层器械可以用。
                                targetEntity = protectTarget;
                            }
                        }
                        targetEntity.Level.ResetHeldItem();
                        UseOnEntity(targetEntity);
                    }
                    break;
                case HeldItemTargetGrid gridTarget:
                    {
                        if (interaction != PointerInteraction.Release)
                            return;

                        var grid = gridTarget.Target;
                        var protector = grid.GetProtectorEntity();
                        var main = grid.GetMainEntity();
                        var carrier = grid.GetCarrierEntity();

                        var protectedEntity = main;
                        if (!CanUseOnEntity(main))
                        {
                            protectedEntity = carrier;
                        }

                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnProtected = CanUseOnEntity(protectedEntity);

                        Entity entity = null;
                        if (canUseOnProtector && canUseOnProtected)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (gridTarget.PointerPosition.y < 0.5f)
                            {
                                entity = protector;
                            }
                            else
                            {
                                entity = protectedEntity;
                            }
                        }
                        else if (canUseOnProtector)
                        {
                            // 只有保护层可以用。
                            entity = protector;
                        }
                        else if (canUseOnProtected)
                        {
                            // 只有主要层器械可以用。
                            entity = protectedEntity;
                        }
                        grid.Level.ResetHeldItem();
                        if (CanUseOnEntity(entity))
                        {
                            UseOnEntity(entity);
                        }
                    }
                    break;
            }
        }
        #endregion

        protected abstract bool CanUseOnEntity(Entity entity);
        protected abstract void UseOnEntity(Entity entity);
    }
}
