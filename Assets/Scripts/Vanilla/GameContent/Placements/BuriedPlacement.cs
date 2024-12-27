using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    [Definition(VanillaPlacementNames.buried)]
    public class BuriedPlacement : ContraptionPlacement
    {
        public BuriedPlacement(string nsp, string name) : base(nsp, name)
        {
        }

        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entityDef, TriggerResultNamespaceID error)
        {
            if (grid.IsWater())
            {
                error.Result = VanillaGridStatus.notOnWater;
                return;
            }
            base.CanPlaceEntityOnGrid(grid, entityDef, error);
        }
    }
}
