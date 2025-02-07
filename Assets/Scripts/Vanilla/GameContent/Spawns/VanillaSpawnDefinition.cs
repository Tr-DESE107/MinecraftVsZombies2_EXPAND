using System.Linq;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

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
            if ((level.CurrentFlag <= 0 && level.CurrentWave <= 3) || !CanSpawnAtWaterLane)
            {
                resultLanes = allLanes.Except(waterLanes);
                if (resultLanes.Count() <= 0)
                {
                    resultLanes = allLanes;
                }
            }
            return level.GetRandomEnemySpawnLane(resultLanes);
        }
        public override int GetWeight(LevelEngine level)
        {
            var weight = this.GetWeightBase();
            var decayStart = this.GetWeightDecayStartFlag();
            var decayEnd = this.GetWeightDecayEndFlag();
            var decay = this.GetWeightDecayPerFlag();

            var decayFlags = Mathf.Clamp(level.CurrentFlag - decayStart, 0, decayEnd);
            return weight - decay * decayFlags;
        }
        public bool CanSpawnAtWaterLane { get; set; }
    }
}
