using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Placements;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    [PlacementDefinition(VanillaPlacementNames.devourer)]
    public class DevourerPlacement : PlacementDefinition
    {
        public DevourerPlacement(string nsp, string name) : base(nsp, name)
        {
        }
        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entity, TriggerResultNamespaceID error)
        {
            var entities = grid.GetEntities();
            if (entities.Count() <= 0 || !entities.Any(e => Devourer.CanMill(e)))
            {
                error.Result = VanillaGridStatus.onlyCanMill;
                return;
            }
            var layersToTake = entity.GetGridLayersToTake();
            var conflictEntities = layersToTake.Select(l => grid.GetLayerEntity(l)).Where(e => e != null);
            if (conflictEntities.Count() > 0)
            {
                error.Result = VanillaGridStatus.alreadyTaken;
                return;
            }
        }
    }
}
