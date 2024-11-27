using System.Linq;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using MVZ2Logic.Level;
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

        public override bool IsForEntity() => !Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnEntity(Entity entity, long id)
        {
            HeldFlags flags = HeldFlags.ForceReset;
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    if (entity.CanTrigger())
                    {
                        flags |= HeldFlags.Valid;
                    }
                    break;
            }
            return flags;
        }
        public override bool UseOnEntity(Entity entity, long id)
        {
            base.UseOnEntity(entity, id);
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Trigger();
                    return true;
            }
            return false;
        }
        public override bool IsForGrid() => Global.IsMobile();
        public override HeldFlags GetHeldFlagsOnGrid(LawnGrid grid, long id)
        {
            var flags = HeldFlags.ForceReset;
            var entities = grid.GetTakenEntities();
            var energy = grid.Level.Energy;
            if (entities.Any(e => e.Type == EntityTypes.PLANT && e.CanTrigger()))
            {
                flags |= HeldFlags.Valid;
            }
            return flags;
        }
        public override bool UseOnGrid(LawnGrid grid, long id)
        {
            base.UseOnGrid(grid, id);
            var entities = grid.GetTakenEntities();
            var energy = grid.Level.Energy;
            var entity = entities.FirstOrDefault(e => e.Type == EntityTypes.PLANT && e.CanTrigger());
            if (entity != null)
            {
                entity.Trigger();
            }
            return true;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, long id)
        {
            base.UseOnLawn(level, area, id);
            if (level.CancelHeldItem() && area == LawnArea.Side)
            {
                level.PlaySound(VanillaSoundID.tap);
            }
        }
    }
}
