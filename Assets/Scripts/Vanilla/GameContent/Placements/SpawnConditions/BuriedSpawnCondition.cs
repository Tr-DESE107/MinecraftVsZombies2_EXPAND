using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class BuriedSpawnCondition : ContraptionSpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            if (grid.IsAir())
                return VanillaGridStatus.notOnAir;
            if (grid.IsSlab())
                return VanillaGridStatus.notOnPlane;
            if (grid.IsWater())
                return VanillaGridStatus.notOnWater;
            return base.GetSpawnError(placement, grid, entity);
        }
    }
}
