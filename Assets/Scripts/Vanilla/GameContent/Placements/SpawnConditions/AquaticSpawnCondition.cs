using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public class AquaticSpawnCondition : ContraptionSpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            if (grid.IsCloud())
                return VanillaGridStatus.notOnAir;

            if (!grid.IsWater())
                return VanillaGridStatus.notOnLand;

            var error = base.GetSpawnError(placement, grid, entity);
            if (error != null)
                return error;

            var carrier = grid.GetLayerEntity(VanillaGridLayers.carrier);
            if (carrier != null)
                return VanillaGridStatus.alreadyTaken;
            return null;
        }
    }
}
