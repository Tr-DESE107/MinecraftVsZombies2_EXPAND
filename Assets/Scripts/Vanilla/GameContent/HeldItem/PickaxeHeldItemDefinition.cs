using System.Linq;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
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
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
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
        public override bool UseOnEntity(Entity entity, long id, PointerPhase phase)
        {
            base.UseOnEntity(entity, id, phase);
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
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            var flags = HeldFlags.ForceReset;
            var entities = grid.GetTakenEntities();
            if (entities.Any(e => e.Type == EntityTypes.PLANT && CanDigContraption(e)))
            {
                flags |= HeldFlags.Valid;
            }
            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, long id, PointerPhase phase)
        {
            base.UseOnGrid(grid, id, phase);
            if (phase != PointerPhase.Release)
                return false;
            var entities = grid.GetTakenEntities();
            var entity = entities.FirstOrDefault(e => e.Type == EntityTypes.PLANT && CanDigContraption(e));
            if (entity != null)
            {
                entity.Die();
            }
            return true;
        }
        #endregion

        public override void UseOnLawn(LevelEngine level, LawnArea area, long id, PointerPhase phase)
        {
            base.UseOnLawn(level, area, id, phase);
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
        private bool CanDigContraption(Entity entity)
        {
            return entity.GetFaction() == entity.Level.Option.LeftFaction;
        }

        public override NamespaceID GetModelID(LevelEngine level, long id)
        {
            return VanillaModelID.pickaxeHeldItem;
        }
    }
}
