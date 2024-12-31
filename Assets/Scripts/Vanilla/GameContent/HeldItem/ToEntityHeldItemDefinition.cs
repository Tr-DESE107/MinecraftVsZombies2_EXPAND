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
    public abstract class ToEntityHeldItemDefinition : HeldItemDefinition
    {
        public ToEntityHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        #region 实体
        public override bool CheckRaycast(HeldItemTarget target)
        {
            switch (target)
            {
                case HeldItemTargetEntity entityTarget:
                    return !Global.IsMobile();
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
                        if (CanUseOnEntity(entity))
                        {
                            return HeldHighlight.Entity;
                        }
                        return HeldHighlight.None;
                    }
                case HeldItemTargetGrid gridTarget:
                    {
                        var grid = gridTarget.Target;

                        var protector = grid.GetProtectorEntity();
                        var main = grid.GetMainEntity();
                        var carrier = grid.GetCarrierEntity();
                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnMain = CanUseOnEntity(main);
                        if (canUseOnProtector && canUseOnMain)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (gridTarget.PointerPosition.y >= 0.5f)
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
                        else if (canUseOnMain)
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
                        else if (protector == null && main == null)
                        {
                            // 保护层和主要层器械不存在，检测承载层。
                            if (CanUseOnEntity(carrier))
                                return HeldHighlight.Green;
                        }
                        return HeldHighlight.Red;
                    }
            }
            return HeldHighlight.None;
        }
        public override void Use(HeldItemTarget target, IHeldItemData data, PointerPhase phase)
        {
            switch (target)
            {
                case HeldItemTargetLawn lawnTarget:
                    {
                        var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
                        if (phase != targetPhase)
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
                        if (phase != PointerPhase.Press)
                            return;
                        var entity = entityTarget.Target;
                        if (CanUseOnEntity(entity))
                        {
                            UseOnEntity(entity);
                        }
                        entity.Level.ResetHeldItem();
                    }
                    break;
                case HeldItemTargetGrid gridTarget:
                    {
                        if (phase != PointerPhase.Release)
                            return;

                        var grid = gridTarget.Target;
                        var protector = grid.GetProtectorEntity();
                        var main = grid.GetMainEntity();
                        var carrier = grid.GetCarrierEntity();
                        var canUseOnProtector = CanUseOnEntity(protector);
                        var canUseOnMain = CanUseOnEntity(main);

                        Entity entity = null;
                        if (canUseOnProtector && canUseOnMain)
                        {
                            // 保护层和主要层器械都可以用，根据光标位置决定。
                            if (gridTarget.PointerPosition.y >= 0.5f)
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
                        else if (protector == null && main == null)
                        {
                            // 保护层和主要层器械不存在，检测承载层。
                            if (CanUseOnEntity(carrier))
                            {
                                entity = carrier;
                            }
                        }
                        if (CanUseOnEntity(entity))
                        {
                            UseOnEntity(entity);
                        }
                        grid.Level.ResetHeldItem();
                    }
                    break;
            }
        }
        #endregion

        protected abstract bool CanUseOnEntity(Entity entity);
        protected abstract void UseOnEntity(Entity entity);
    }
}
