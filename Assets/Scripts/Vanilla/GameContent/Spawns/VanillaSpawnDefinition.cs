using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Spawns
{
    public class VanillaSpawnDefinition : SpawnDefinition
    {
        public VanillaSpawnDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public void SetBehaviours(ISpawnPreviewBehaviour preview, ISpawnInLevelBehaviour inLevel, ISpawnEndlessBehaviour endless)
        {
            previewBehaviour = preview;
            inLevelBehaviour = inLevel;
            endlessBehaviour = endless;
        }
        protected override ISpawnPreviewBehaviour GetPreviewBehaviour() => previewBehaviour;
        protected override ISpawnInLevelBehaviour GetInLevelBehaviour() => inLevelBehaviour;
        protected override ISpawnEndlessBehaviour GetEndlessBehaviour() => endlessBehaviour;
        private ISpawnInLevelBehaviour inLevelBehaviour;
        private ISpawnPreviewBehaviour previewBehaviour;
        private ISpawnEndlessBehaviour endlessBehaviour;
    }
    public class SpawnInLevelBehaviour : ISpawnInLevelBehaviour
    {
        public SpawnInLevelBehaviour(SpawnDefinition definition, int spawnLevel, NamespaceID entityID, bool water, bool air)
        {
            SpawnLevel = spawnLevel;
            EntityID = entityID;
            CanSpawnAtWaterLane = water;
            CanSpawnAtAirLane = air;
            Definition = definition;
        }

        public void PreSpawnAtWave(LevelEngine level, int wave, ref float totalPoints)
        {
        }
        public int GetRandomSpawnLane(LevelEngine level)
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
        public bool CanSpawnInLevel(LevelEngine level)
        {
            return GetSpawnLevel(level) > 0;
        }
        public int GetWeight(LevelEngine level)
        {
            var weight = Definition.GetWeightBase();
            var decayStart = Definition.GetWeightDecayStartFlag();
            var decayEnd = Definition.GetWeightDecayEndFlag();
            var decay = Definition.GetWeightDecayPerFlag();

            var decayFlags = Mathf.Clamp(level.CurrentFlag, decayStart, decayEnd) - decayStart;
            return weight - decay * decayFlags;
        }
        public int GetSpawnLevel(LevelEngine level) => SpawnLevel;
        public NamespaceID GetSpawnEntityID() => EntityID;
        public int SpawnLevel { get; }
        public NamespaceID EntityID { get; }
        public bool CanSpawnAtWaterLane { get; }
        public bool CanSpawnAtAirLane { get; }
        public SpawnDefinition Definition { get; }
    }
    public class SpawnPreviewBehaviour : ISpawnPreviewBehaviour
    {
        public SpawnPreviewBehaviour(NamespaceID entityID)
        {
            EntityID = entityID;
        }
        public NamespaceID GetPreviewEntityID()
        {
            return EntityID;
        }
        public NamespaceID[] GetCounterTags(LevelEngine level)
        {
            var entityID = EntityID;
            var entityDef = level.Content.GetEntityDefinition(entityID);
            if (entityDef == null)
                return null;
            return entityDef.GetCounterTags();
        }
        public NamespaceID EntityID { get; }
    }
    public class SpawnEndlessBehaviour : ISpawnEndlessBehaviour
    {
        public SpawnEndlessBehaviour(bool noEndless, NamespaceID[] excludedAreaTags)
        {
            NoEndless = noEndless;
            ExcludedAreaTags = excludedAreaTags;
        }
        public bool CanAppearInEndless(LevelEngine level)
        {
            if (NoEndless)
                return false;
            var excludedAreaTags = ExcludedAreaTags;
            var areaDef = level.AreaDefinition;
            if (areaDef.GetAreaTags().Any(t => excludedAreaTags.Contains(t)))
                return false;
            return true;
        }
        public bool NoEndless { get; }
        public NamespaceID[] ExcludedAreaTags { get; }
    }
}
