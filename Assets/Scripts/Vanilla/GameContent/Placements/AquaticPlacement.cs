using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.aquatic)]
    public class AquaticPlacement : ContraptionPlacement
    {
        public AquaticPlacement(string nsp, string name) : base(nsp, name)
        {
        }

        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entityDef, TriggerResultNamespaceID error)
        {
            if (!grid.IsWater())
            {
                error.Result = VanillaGridStatus.notOnLand;
                return;
            }
            var carrier = grid.GetLayerEntity(VanillaGridLayers.carrier);
            if (carrier != null)
            {
                error.Result = VanillaGridStatus.alreadyTaken;
                return;
            }
            base.CanPlaceEntityOnGrid(grid, entityDef, error);
        }
    }
}
