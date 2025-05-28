using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class NormalSpawnCondition : ContraptionSpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            if (grid.IsWater())
            {
                var carrier = grid.GetLayerEntity(VanillaGridLayers.carrier);
                if (carrier == null)
                {
                    return VanillaGridStatus.needLilypad;
                }
            }
            return base.GetSpawnError(placement, grid, entity);
        }
    }
}
