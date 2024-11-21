using System.Linq;
using MVZ2.Definitions;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.HeldItems;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.pickaxe)]
    public class PickaxeHeldItemDefinition : HeldItemDefinition
    {
        public PickaxeHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

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
        public override bool UseOnEntity(Entity entity, long id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Die();
                    return true;
            }
            return false;
        }
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
        public override bool UseOnGrid(LawnGrid grid, long id)
        {
            base.UseOnGrid(grid, id);
            var entities = grid.GetTakenEntities();
            var entity = entities.FirstOrDefault(e => e.Type == EntityTypes.PLANT && CanDigContraption(e));
            if (entity != null)
            {
                entity.Die();
            }
            return true;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, long id)
        {
            base.UseOnLawn(level, area, id);
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(SoundID.tap);
            }
        }
        private bool CanDigContraption(Entity entity)
        {
            return entity.GetFaction() == entity.Level.Option.LeftFaction;
        }
        public override bool IsForGrid() => Global.IsMobile();
        public override bool IsForEntity() => !Global.IsMobile();
    }
}
