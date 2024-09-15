using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.pickaxe)]
    public class PickaxeHeldItemDefinition : HeldItemDefinition
    {
        public PickaxeHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    return CanDigContraption(entity);
            }
            return false;
        }
        public override bool IsValidOnGrid(LawnGrid grid, int id)
        {
            return false;
        }
        public override bool UseOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Die();
                    return true;
            }
            return false;
        }
        public override void UseOnLawn(LevelEngine level, LawnArea area, int id)
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
    }
}
