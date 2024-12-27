using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Placements
{
    public abstract class ContraptionPlacement : PlacementDefinition
    {
        protected ContraptionPlacement(string nsp, string name) : base(nsp, name)
        {
        }

        public override void CanPlaceEntityOnGrid(LawnGrid grid, EntityDefinition entityDef, TriggerResultNamespaceID error)
        {
            var layersToTake = entityDef.GetGridLayersToTake();
            var conflictEntities = layersToTake.Select(l => grid.GetLayerEntity(l)).Where(e => e != null);
            if (conflictEntities.Count() > 0)
            {
                if (conflictEntities.Any(e => e.IsEntityOf(VanillaObstacleID.gargoyleStatue)))
                {
                    error.Result = VanillaGridStatus.notOnStatues;
                    return;
                }
                error.Result = VanillaGridStatus.alreadyTaken;
                return;
            }
        }
    }
}
