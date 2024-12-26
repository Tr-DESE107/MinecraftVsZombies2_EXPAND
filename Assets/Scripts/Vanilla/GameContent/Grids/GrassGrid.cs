using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Grids
{
    [Definition(VanillaGridNames.grass)]
    public class GrassGrid : GridDefinition
    {
        public GrassGrid(string nsp, string name) : base(nsp, name)
        {
        }
        public override void CanPlaceEntity(LawnGrid grid, NamespaceID entityID, TriggerResultNamespaceID error)
        {
            var level = grid.Level;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef.Type == EntityTypes.PLANT)
            {
                var gridDefID = grid.Definition.GetID();
                if (gridDefID == VanillaGridID.grass)
                {
                    if (!entityDef.CanPlaceOnLand())
                    {
                        error.Result = VanillaGridStatus.notOnLand;
                        return;
                    }
                }

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
}
