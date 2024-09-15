using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    [Definition(HeldItemNames.starshard)]
    public class StarshardHeldItemDefinition : HeldItemDefinition
    {
        public StarshardHeldItemDefinition(string nsp, string name) : base(nsp, name)
        {
        }

        public override bool IsValidOnEntity(Entity entity, int id)
        {
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    return entity.CanEvoke();
            }
            return false;
        }
        public override bool UseOnEntity(Entity entity, int id)
        {
            base.UseOnEntity(entity, id);
            switch (entity.Type)
            {
                case EntityTypes.PLANT:
                    entity.Evoke();
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
        public override bool IsValidOnGrid(LawnGrid grid, int id)
        {
            return false;
        }
    }
}
