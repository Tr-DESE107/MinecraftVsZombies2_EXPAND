using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class EnemyPlaceMethod : EntityPlaceMethod
    {
        public override NamespaceID GetPlaceError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var level = grid.Level;
            if (level.IsIZombie())
            {
                var lane = grid.Lane;
                var line = level.FindFirstEntityWithTheLeast(e => e.IsEntityOf(VanillaEffectID.redline) && e.GetLane() == lane, e => e.Position.x);
                if (line != null)
                {
                    var column = grid.Column;
                    var gridX = level.GetEntityColumnX(column);
                    if (gridX < line.Position.x)
                    {
                        return VanillaGridStatus.rightOfLine;
                    }
                }
            }
            return base.GetPlaceError(placement, grid, entity);
        }
    }
}
