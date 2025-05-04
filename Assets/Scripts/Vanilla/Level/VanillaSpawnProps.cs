using PVZEngine;
using PVZEngine.Definitions;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion]
    public static class VanillaSpawnProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta MIN_SPAWN_WAVE = Get("minSpawnWave");
        public static int GetMinSpawnWave(this SpawnDefinition def)
        {
            return def.GetProperty<int>(MIN_SPAWN_WAVE);
        }
        public static readonly PropertyMeta PREVIEW_COUNT = Get("previewCount");
        public static int GetPreviewCount(this SpawnDefinition def)
        {
            return def.TryGetProperty(PREVIEW_COUNT, out int value) ? value : 1;
        }
        public static readonly PropertyMeta WEIGHT_BASE = Get("weightBase");
        public static readonly PropertyMeta WEIGHT_DECAY_START = Get("weightDecayStart");
        public static readonly PropertyMeta WEIGHT_DECAY_END = Get("weightDecayEnd");
        public static readonly PropertyMeta WEIGHT_DECAY = Get("weightDecay");
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
