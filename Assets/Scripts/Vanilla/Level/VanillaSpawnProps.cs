using PVZEngine.Definitions;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaSpawnProps
    {
        public const string PREVIEW_COUNT = "previewCount";
        public static int GetPreviewCount(this SpawnDefinition def)
        {
            return def.TryGetProperty(PREVIEW_COUNT, out int value) ? value : 1;
        }
        public const string WEIGHT_BASE = "weightBase";
        public const string WEIGHT_DECAY_START = "weightDecayStart";
        public const string WEIGHT_DECAY_END = "weightDecayEnd";
        public const string WEIGHT_DECAY = "weightDecay";
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
