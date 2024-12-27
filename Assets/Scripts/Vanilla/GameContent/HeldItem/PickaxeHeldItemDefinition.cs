using System.Linq;
using MVZ2.GameContent.Damages;
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
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.HeldItems
{
    [Definition(VanillaHeldItemNames.pickaxe)]
    public class PickaxeHeldItemDefinition : HeldItemDefinition
    {
        public PickaxeHeldItemDefinition(string nsp, string name) : base(nsp, name)
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
                    if (CanDigContraption(entity))
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
                    var effects = new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE);
                    entity.Die(new DamageInput(0, effects, entity, new EntityReferenceChain(null)));
                    return true;
            }
            return false;
        }
        #endregion

        #region 网格
        public override bool IsForGrid() => Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, IHeldItemData data)
        {
            var flags = HeldFlags.ForceReset;

            var protector = grid.GetProtectorEntity();
            var main = grid.GetMainEntity();
            var carrier = grid.GetCarrierEntity();
            if (protector != null && protector.Type == EntityTypes.PLANT && CanDigContraption(protector))
                flags |= HeldFlags.ValidOnProtector;

            if (main != null && main.Type == EntityTypes.PLANT && CanDigContraption(main))
                flags |= HeldFlags.Valid;

            if (carrier != null && carrier.Type == EntityTypes.PLANT && CanDigContraption(carrier))
                flags |= HeldFlags.ValidOnCarrier;

            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, IHeldItemData data, PointerPhase phase, NamespaceID targetLayer)
        {
            base.UseOnGrid(grid, data, phase, targetLayer);
            if (phase != PointerPhase.Release)
                return false;
            var entity = grid.GetLayerEntity(targetLayer);
            if (entity != null && entity.Type == EntityTypes.PLANT && CanDigContraption(entity))
            {
                entity.Die();
            }
            return true;
        }
        #endregion

        public override void UseOnLawn(LevelEngine level, LawnArea area, IHeldItemData data, PointerPhase phase)
        {
            base.UseOnLawn(level, area, data, phase);
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private bool CanDigContraption(Entity entity)
        {
            if (entity.HasPassenger())
            {
                return false;
            }
            return entity.GetFaction() == entity.Level.Option.LeftFaction;
        }

        public override NamespaceID GetModelID(LevelEngine level, long id)
        {
            return VanillaModelID.pickaxeHeldItem;
        }
    }
}
