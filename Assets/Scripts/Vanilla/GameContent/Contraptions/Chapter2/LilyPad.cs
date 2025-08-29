using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.lilyPad)]
    public class LilyPad : ContraptionBehaviour
    {
        public LilyPad(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            var level = entity.Level;
            var column = entity.GetColumn();
            var lane = entity.GetLane();
            for (int x = column - 1; x <= column + 1; x++)
            {
                for (int y = lane - 1; y <= lane + 1; y++)
                {
                    var grid = level.GetGrid(x, y);
                    if (grid == null || !grid.IsWater() || !grid.IsEmpty())
                        continue;
                    var lily = level.Spawn(VanillaContraptionID.lilyPad, grid.GetEntityPosition(), entity);
                    lily.AddBuff<LilyPadEvocationBuff>();
                    lily.PlaySplashEffect();
                }
            }
            level.PlaySound(VanillaSoundID.water);
        }
    }
}
