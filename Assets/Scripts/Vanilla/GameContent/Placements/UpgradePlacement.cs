using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Placements;
using PVZEngine.Callbacks;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.upgrade)]
    public class UpgradePlacement : PlacementDefinition
    {
        public UpgradePlacement(string nsp, string name) : base(nsp, name)
        {
        }
        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entity, CallbackResult error)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => e.IsEntityOf(entity.GetUpgradeFromEntity()) && e.IsFriendlyEntity()))
            {
                error.SetFinalValue(VanillaGridStatus.onlyUpgrade);
                return;
            }
        }
    }
}
