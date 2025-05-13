namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class EngineStageProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta CONTINUED_FIRST_WAVE_TIME = Get("continuedFirstWaveTime");
        public static readonly PropertyMeta FIRST_WAVE_TIME = Get("firstWaveTime");
        public static readonly PropertyMeta WAVES_PER_FLAG = Get("wavesPerFlag");
        public static readonly PropertyMeta TOTAL_FLAGS = Get("totalFlags");
        public static int GetTotalFlags(this LevelEngine level)
        {
            return level.GetProperty<int>(TOTAL_FLAGS);
        }
        public static int GetWavesPerFlag(this LevelEngine level)
        {
            return level.GetProperty<int>(WAVES_PER_FLAG);
        }
        public static int GetFirstWaveTime(this LevelEngine level)
        {
            return level.GetProperty<int>(FIRST_WAVE_TIME);
        }
        public static int GetContinutedFirstWaveTime(this LevelEngine level)
        {
            return level.GetProperty<int>(CONTINUED_FIRST_WAVE_TIME);
        }
        public static bool IsHugeWave(this LevelEngine level, int wave)
        {
            return wave > 0 && wave % level.GetWavesPerFlag() == 0;
        }
        public static int GetTotalWaveCount(this LevelEngine level)
        {
            return level.GetTotalFlags() * level.GetWavesPerFlag();
        }
        public static bool IsFinalWave(this LevelEngine level, int wave)
        {
            return wave == level.GetTotalWaveCount();
        }
    }
}
