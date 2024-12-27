using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.GameContent.Spawns
{
    public class VanillaSpawnDefinition : SpawnDefinition
    {
        public VanillaSpawnDefinition(string nsp, string name, int cost, NamespaceID entityID, NamespaceID[] excludedAreaTags) : base(nsp, name, cost, entityID, excludedAreaTags)
        {
        }

        public override int GetRandomSpawnLane(LevelEngine level)
        {
            var allLanes = level.GetAllLanes();
            var waterLanes = level.GetWaterLanes();
            var resultLanes = allLanes;
            if (level.CurrentWave <= 3 || !CanSpawnAtWaterLane)
            {
                resultLanes = allLanes.Except(waterLanes);
                if (resultLanes.Count() <= 0)
                {
                    resultLanes = allLanes;
                }
            }
            return level.GetRandomEnemySpawnLane(resultLanes);
        }
        public bool CanSpawnAtWaterLane { get; set; }
    }
}
