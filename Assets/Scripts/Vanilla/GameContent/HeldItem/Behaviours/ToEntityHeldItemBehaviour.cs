using System.Linq;
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
                        var protectTargets = entity.GetProtectingTargets();
                        if (protectTargets == null || protectTargets.Length <= 0)
                        {
                            // 没有保护中的目标：
                            // 可能是没有保护的器械，也可能是不能保护器械。
                            // 直接选中当前器械。
                            return HeldHighlight.Entity(entity);
                        }

                        // 存在保护中的目标。
                        var canUseOnProtector = CanUseOnEntity(protector);
                        var firstValidMainTarget = protectTargets.FirstOrDefault(t => CanUseOnEntity(t));
                        var canUseOnMain = firstValidMainTarget != null;

                        // 可以给保护层器械使用，并且光标位置位于下方，或者内部器械不能使用。
                        if (canUseOnProtector && (!canUseOnMain || entityTarget.PointerPosition.y < 0.5f))
                        {
                            return HeldHighlight.Entity(entity);
                        }
                        // 主要层器械可以用。
                        if (canUseOnMain)
                        {
                            return HeldHighlight.Entity(firstValidMainTarget);
                        }
                        // 没有能用的目标。
                        return HeldHighlight.None;
                    }
                case HeldItemTargetGrid gridTarget:
                    {
                        var grid = gridTarget.Target;

                        var game = Global.Game;
                        var protector = grid.GetProtectorEntity();
                        var protectedLayers = VanillaGridLayers.protectedLayers;

                        var main = protectedLayers
                            .Select(t => grid.GetLayerEntity(t))
                            .FirstOrDefault(t => CanUseOnEntity(t));

                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnMain = CanUseOnEntity(main);

                        // 保护层器械和内部都可以用。
                        if (canUseOnProtector && canUseOnMain)
                        {
                            // 根据光标位置决定。
                            if (gridTarget.PointerPosition.y < 0.5f)
                            {
                                return HeldHighlight.Green(0, 0.5f);
                            }
                            else
                            {
                                return HeldHighlight.Green(0.5f, 1f);
                            }
                        }
                        else if (canUseOnProtector) // 只有保护层器械可以用。
                        {
                            // 没有内部器械。
                            if (main == null)
                            {
                                return HeldHighlight.Green();
                            }
                            // 内部的器械存在，但是不可以使用。
                            return HeldHighlight.Green(0, 0.5f);
                        }
                        if (canUseOnMain) // 只有内部器械可以用。
                        {
                            // 没有保护层器械。
                            if (protector == null)
                            {
                                return HeldHighlight.Green();
                            }
                            // 保护层器械存在，但是不可以使用。
                            return HeldHighlight.Green(0.5f, 1f);
                        }
                        return HeldHighlight.Red();
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
                        var protectTargets = entity.GetProtectingTargets();
                        if (protectTargets != null && protectTargets.Length > 0)
                        {
                            var canUseOnProtector = CanUseOnEntity(protector);
                            var firstValidMainTarget = protectTargets.FirstOrDefault(t => CanUseOnEntity(t));
                            var canUseOnMain = firstValidMainTarget != null;
                            if (canUseOnProtector && canUseOnMain)
                            {
                                // 保护层和主要层器械都可以用，根据光标位置决定。
                                if (entityTarget.PointerPosition.y >= 0.5f)
                                {
                                    targetEntity = firstValidMainTarget;
                                }
                            }
                            else if (!canUseOnProtector)
                            {
                                // 只有主要层器械可以用。
                                targetEntity = firstValidMainTarget;
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
                        var game = Global.Game;
                        var protector = grid.GetProtectorEntity();
                        var protectedLayers = VanillaGridLayers.protectedLayers;

                        var main = protectedLayers
                            .Select(t => grid.GetLayerEntity(t))
                            .FirstOrDefault(t => CanUseOnEntity(t));

                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnMain = CanUseOnEntity(main);

                        Entity entity = null;
                        if (canUseOnProtector && canUseOnMain)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (gridTarget.PointerPosition.y < 0.5f)
                            {
                                entity = protector;
                            }
                            else
                            {
                                entity = main;
                            }
                        }
                        else if (canUseOnProtector)
                        {
                            // 只有保护层可以用。
                            entity = protector;
                        }
                        else if (canUseOnMain)
                        {
                            // 只有主要层器械可以用。
                            entity = main;
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
