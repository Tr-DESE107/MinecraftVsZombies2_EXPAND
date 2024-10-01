namespace PVZEngine.Level
{
    public static class EngineStageProps
    {
        public const string CONTINUED_FIRST_WAVE_TIME = "continuedFirstWaveTime";
        public const string FIRST_WAVE_TIME = "firstWaveTime";
        public const string WAVES_PER_FLAG = "wavesPerFlag";
        public const string TOTAL_FLAGS = "totalFlags";
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
