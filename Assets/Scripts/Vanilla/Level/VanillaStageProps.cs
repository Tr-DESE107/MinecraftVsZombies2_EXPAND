using System.Linq;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Level
{
    public static class VanillaStageProps
    {
        public const string LEVEL_NAME = "levelName";
        public const string DAY_NUMBER = "dayNumber";

        public const string WAVE_MAX_TIME = "waveMaxTime";
        public const string WAVE_ADVANCE_TIME = "waveAdvanceTime";
        public const string WAVE_ADVANCE_HEALTH_PERCENT = "waveAdvanceHealthPercent";

        public const string NO_PRODUCTION = "noProduction";
        public const string AUTO_COLLECT = "autoCollect";

        public const string TALKS = "talks";

        public const string CLEAR_PICKUP_MODEL = "clearPickupModel";
        public const string CLEAR_PICKUP_BLUEPRINT = "clearPickupBlueprint";
        public const string END_NOTE_ID = "endNoteId";

        public const string START_TRANSITION = "startTransition";
        public const string START_CAMERA_POSITION = "startCameraPosition";

        public const string SPAWN_POINTS_MULTIPLIER = "SpawnPointsMultiplier";

        public static NamespaceID GetClearPickupModel(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(CLEAR_PICKUP_MODEL);
        }
        public static NamespaceID GetClearPickupBlueprint(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(CLEAR_PICKUP_BLUEPRINT);
        }
        public static string GetLevelName(this LevelEngine level)
        {
            return level.GetProperty<string>(LEVEL_NAME);
        }
        public static void SetLevelName(this StageDefinition stage, string name)
        {
            stage.SetProperty(LEVEL_NAME, name);
        }
        public static int GetDayNumber(this LevelEngine level)
        {
            return level.GetProperty<int>(DAY_NUMBER);
        }
        public static void SetDayNumber(this StageDefinition stage, int number)
        {
            stage.SetProperty(DAY_NUMBER, number);
        }
        public static bool IsAutoCollect(this LevelEngine game)
        {
            return game.GetProperty<bool>(AUTO_COLLECT);
        }
        public static bool IsNoProduction(this LevelEngine game)
        {
            return game.GetProperty<bool>(NO_PRODUCTION);
        }
        public static void SetNoProduction(this LevelEngine game, bool value)
        {
            game.SetProperty(NO_PRODUCTION, value);
        }
        public static IStageTalkMeta[] GetTalks(this LevelEngine game)
        {
            return game.GetProperty<IStageTalkMeta[]>(TALKS);
        }
        public static IStageTalkMeta GetTalk(this LevelEngine game, string type)
        {
            var talks = game.GetTalks();
            if (talks == null)
                return null;
            return talks.FirstOrDefault(t => t.Type == type);
        }
        public static NamespaceID GetEndNoteID(this LevelEngine game)
        {
            return game.GetProperty<NamespaceID>(END_NOTE_ID);
        }
        public static string GetStartTransition(this StageDefinition stage)
        {
            return stage.GetProperty<string>(START_TRANSITION);
        }
        public static LevelCameraPosition GetStartCameraPosition(this StageDefinition stage)
        {
            return (LevelCameraPosition)stage.GetProperty<int>(START_CAMERA_POSITION);
        }
        public static void SetSpawnPointMultiplier(this StageDefinition stage, float value)
        {
            stage.SetProperty(SPAWN_POINTS_MULTIPLIER, value);
        }
        public static float GetSpawnPointMultiplier(this LevelEngine level)
        {
            return level.GetProperty<float>(SPAWN_POINTS_MULTIPLIER);
        }
    }
}
