using System.Linq;
using MVZ2.GameContent.Obstacles;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Placements;

namespace MVZ2.GameContent.Placements
{
    public abstract class ContraptionSpawnCondition : SpawnCondition
    {
        public override NamespaceID GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity)
        {
            var layersToTake = entity.GetGridLayersToTake();
            var conflictEntities = layersToTake.Select(l => grid.GetLayerEntity(l)).Where(e => e != null);
            if (conflictEntities.Count() > 0)
            {
                foreach (var ent in conflictEntities)
                {
                    if (ent.IsEntityOf(VanillaObstacleID.gargoyleStatue))
                    {
                        return VanillaGridStatus.notOnStatues;
                    }
                    if (ent.IsEntityOf(VanillaObstacleID.monsterSpawner))
                    {
                        return VanillaGridStatus.notOnSpawners;
                    }
                }
                return VanillaGridStatus.alreadyTaken;
            }
            return null;
        }
    }
}
