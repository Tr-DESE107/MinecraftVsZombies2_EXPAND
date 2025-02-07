using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Placements;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.normal)]
    public class NormalPlacement : ContraptionPlacement
    {
        public NormalPlacement(string nsp, string name) : base(nsp, name)
        {
        }

        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entityDef, TriggerResultNamespaceID error)
        {
            if (grid.IsWater())
            {
                var carrier = grid.GetLayerEntity(VanillaGridLayers.carrier);
                if (carrier == null)
                {
                    error.Result = VanillaGridStatus.needLilypad;
                    return;
                }
            }
            base.CanPlaceEntityOnGrid(grid, entityDef, error);
        }
    }
}
