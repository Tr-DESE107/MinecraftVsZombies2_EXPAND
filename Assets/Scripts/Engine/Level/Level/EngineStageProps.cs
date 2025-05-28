namespace PVZEngine.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class EngineStageProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }

        public static readonly PropertyMeta<int> TOTAL_FLAGS = Get<int>("totalFlags");
        public static int GetTotalFlags(this LevelEngine level)
        {
            return level.GetProperty<int>(TOTAL_FLAGS);
        }

        public static readonly PropertyMeta<int> WAVES_PER_FLAG = Get<int>("wavesPerFlag");
        public static int GetWavesPerFlag(this LevelEngine level)
        {
            return level.GetProperty<int>(WAVES_PER_FLAG);
        }

        public static readonly PropertyMeta<int> FIRST_WAVE_TIME = Get<int>("firstWaveTime");
        public static int GetFirstWaveTime(this LevelEngine level)
        {
            return level.GetProperty<int>(FIRST_WAVE_TIME);
        }

        public static readonly PropertyMeta<int> CONTINUED_FIRST_WAVE_TIME = Get<int>("continuedFirstWaveTime");
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
