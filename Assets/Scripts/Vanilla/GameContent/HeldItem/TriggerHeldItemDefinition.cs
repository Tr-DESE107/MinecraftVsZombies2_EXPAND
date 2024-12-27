using MVZ2.GameContent.Models;
using MVZ2.HeldItems;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.trigger)]
    public class TriggerHeldItemDefinition : HeldItemDefinition
    {
        public TriggerHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        #region 实体
        public override bool IsForEntity() => !Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, IHeldItemData data)
        {
            HeldFlags flags = HeldFlags.ForceReset;
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    if (CanTrigger(entity))
                    {
                        flags |= HeldFlags.Valid;
                    }
                    break;
            }
            return flags;
        }
        public override bool UseOnEntity(Entity entity, IHeldItemData data, PointerPhase phase)
        {
            base.UseOnEntity(entity, data, phase);
            if (phase != PointerPhase.Press)
                return false;
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Trigger();
                    return true;
            }
            return false;
        }
        #endregion

        #region 图格
        public override bool IsForGrid() => Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data)
        {
            var flags = HeldFlags.ForceReset;

            var protector = grid.GetProtectorEntity();
            var main = grid.GetMainEntity();
            var carrier = grid.GetCarrierEntity();
            if (protector != null && protector.Type == EntityTypes.PLANT && CanTrigger(protector))
                flags |= HeldFlags.ValidOnProtector;

            if (main != null && main.Type == EntityTypes.PLANT && CanTrigger(main))
                flags |= HeldFlags.Valid;

            if (carrier != null && carrier.Type == EntityTypes.PLANT && CanTrigger(carrier))
                flags |= HeldFlags.ValidOnCarrier;

            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, IHeldItemData data, PointerPhase phase, NamespaceID targetLayer)
        {
            base.UseOnGrid(grid, data, phase, targetLayer);
            if (phase != PointerPhase.Release)
                return false;
            var entity = grid.GetLayerEntity(targetLayer);
            if (entity != null && entity.Type == EntityTypes.PLANT && CanTrigger(entity))
            {
                entity.Trigger();
            }
            return true;
        }
        #endregion
        public override void UseOnLawn(LevelEngine level, LawnArea area, IHeldItemData data, PointerPhase phase)
        {
            base.UseOnLawn(level, area, data, phase);
            var targetPhase = Global.IsMobile() ? PointerPhase.Release : PointerPhase.Press;
            if (phase != targetPhase)
                return;
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }

        public override NamespaceID GetModelID(LevelEngine level, long id)
        {
            return VanillaModelID.triggerHeldItem;
        }
        private bool CanTrigger(Entity entity)
        {
            if (entity.HasPassenger())
            {
                return false;
            }
            return entity.GetFaction() == entity.Level.Option.LeftFaction && entity.CanTrigger();
        }
    }
}
