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
        public VanillaSpawnDefinition(string nsp, string name, int cost, bool noEndless, NamespaceID entityID, NamespaceID[] excludedAreaTags) : base(nsp, name, cost, noEndless, entityID, excludedAreaTags)
        {
        }

        public override int GetRandomSpawnLane(LevelEngine level)
        {
            var allLanes = level.GetAllLanes();
            var resultLanes = allLanes;

            bool isStartWaves = level.CurrentFlag <= 0 && level.CurrentWave <= 3;
            if (isStartWaves || !CanSpawnAtWaterLane)
            {
                var waterLanes = level.GetWaterLanes();
                resultLanes = resultLanes.Except(waterLanes);
            }
            if (isStartWaves || !CanSpawnAtAirLane)
            {
                var airLanes = level.GetAirLanes();
                resultLanes = resultLanes.Except(airLanes);
            }

            if (resultLanes.Count() <= 0)
            {
                resultLanes = allLanes;
            }
            return level.GetRandomEnemySpawnLane(resultLanes);
        }
        public override int GetWeight(LevelEngine level)
        {
            var weight = this.GetWeightBase();
            var decayStart = this.GetWeightDecayStartFlag();
            var decayEnd = this.GetWeightDecayEndFlag();
            var decay = this.GetWeightDecayPerFlag();

            var decayFlags = Mathf.Clamp(level.CurrentFlag, decayStart, decayEnd) - decayStart;
            return weight - decay * decayFlags;
        }
        public bool CanSpawnAtWaterLane { get; set; }
        public bool CanSpawnAtAirLane { get; set; }
    }
}
