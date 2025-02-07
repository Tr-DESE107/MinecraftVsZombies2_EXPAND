using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Placements;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.dreamSilk)]
    public class DreamSilkPlacement : PlacementDefinition
    {
        public DreamSilkPlacement(string nsp, string name) : base(nsp, name)
        {
        }
        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entity, TriggerResultNamespaceID error)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => DreamSilk.CanSleep(e)))
            {
                error.Result = VanillaGridStatus.onlyCanSleep;
                return;
            }
        }
    }
}
