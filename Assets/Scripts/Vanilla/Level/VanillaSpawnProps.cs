using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion(PropertyRegions.spawn)]
    public static class VanillaSpawnProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<int> MIN_SPAWN_WAVE = Get<int>("minSpawnWave");
        public static int GetMinSpawnWave(this SpawnDefinition def)
        {
            return def.GetProperty<int>(MIN_SPAWN_WAVE);
        }
        public static readonly PropertyMeta<int> PREVIEW_COUNT = Get<int>("previewCount");
        public static int GetPreviewCount(this SpawnDefinition def)
        {
            return def.TryGetProperty(PREVIEW_COUNT, out int value) ? value : 1;
        }
        public static readonly PropertyMeta<int> WEIGHT_BASE = Get<int>("weightBase");
        public static readonly PropertyMeta<int> WEIGHT_DECAY_START = Get<int>("weightDecayStart");
        public static readonly PropertyMeta<int> WEIGHT_DECAY_END = Get<int>("weightDecayEnd");
        public static readonly PropertyMeta<int> WEIGHT_DECAY = Get<int>("weightDecay");
        public static int GetWeightBase(this SpawnDefinition def)
        {
            return def.GetProperty<int>(WEIGHT_BASE);
        }
        public static int GetWeightDecayStartFlag(this SpawnDefinition def)
        {
            return def.GetProperty<int>(WEIGHT_DECAY_START);
        }
        public static int GetWeightDecayEndFlag(this SpawnDefinition def)
        {
            return def.GetProperty<int>(WEIGHT_DECAY_END);
        }
        public static int GetWeightDecayPerFlag(this SpawnDefinition def)
        {
            return def.GetProperty<int>(WEIGHT_DECAY);
        }
    }
}
