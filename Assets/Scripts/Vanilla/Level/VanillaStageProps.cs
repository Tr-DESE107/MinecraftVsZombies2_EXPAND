using System.Linq;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion]
    public static class VanillaStageProps
    {
        private static PropertyMeta Get(string name)
        {
            return new PropertyMeta(name);
        }
        public static readonly PropertyMeta LEVEL_NAME = Get("levelName");
        public static readonly PropertyMeta DAY_NUMBER = Get("dayNumber");

        public static readonly PropertyMeta WAVE_MAX_TIME = Get("waveMaxTime");
        public static readonly PropertyMeta WAVE_ADVANCE_TIME = Get("waveAdvanceTime");
        public static readonly PropertyMeta WAVE_ADVANCE_HEALTH_PERCENT = Get("waveAdvanceHealthPercent");

        public static readonly PropertyMeta NO_PRODUCTION = Get("noProduction");
        public static readonly PropertyMeta AUTO_COLLECT = Get("autoCollect");

        public static readonly PropertyMeta TALKS = Get("talks");

        public static readonly PropertyMeta CLEAR_PICKUP_MODEL = Get("clearPickupModel");
        public static readonly PropertyMeta CLEAR_PICKUP_BLUEPRINT = Get("clearPickupBlueprint");
        public static readonly PropertyMeta END_NOTE_ID = Get("endNoteId");

        public static readonly PropertyMeta START_TRANSITION = Get("startTransition");
        public static readonly PropertyMeta START_CAMERA_POSITION = Get("startCameraPosition");

        public static readonly PropertyMeta NEED_BLUEPRINTS = Get("needBlueprints");
        public static readonly PropertyMeta CLEAR_SOUND = Get("clearSound");

        public static readonly PropertyMeta ENDLESS = Get("endless");


        public static int GetWaveMaxTime(this LevelEngine level) => level.GetProperty<int>(VanillaStageProps.WAVE_MAX_TIME);
        public static int GetWaveAdvanceTime(this LevelEngine level) => level.GetProperty<int>(VanillaStageProps.WAVE_ADVANCE_TIME);
        public static float GetWaveAdvanceHealthPercent(this LevelEngine level) => level.GetProperty<float>(VanillaStageProps.WAVE_ADVANCE_HEALTH_PERCENT);
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
        public static string GetLevelName(this StageDefinition stage)
        {
            return stage.GetProperty<string>(LEVEL_NAME);
        }
        public static void SetLevelName(this StageDefinition stage, string name)
        {
            stage.SetProperty(LEVEL_NAME, name);
        }
        public static int GetDayNumber(this LevelEngine level)
        {
            return level.GetProperty<int>(DAY_NUMBER);
        }
        public static int GetDayNumber(this StageDefinition stage)
        {
            return stage.GetProperty<int>(DAY_NUMBER);
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
        public static void SetNeedBlueprints(this StageDefinition stage, bool value)
        {
            stage.SetProperty(NEED_BLUEPRINTS, value);
        }
        public static bool NeedBlueprints(this LevelEngine level)
        {
            return level.GetProperty<bool>(NEED_BLUEPRINTS);
        }
        public static void SetClearSound(this StageDefinition stage, NamespaceID value)
        {
            stage.SetProperty(CLEAR_SOUND, value);
        }
        public static NamespaceID GetClearSound(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID>(CLEAR_SOUND) ?? VanillaSoundID.winMusic;
        }
        public static bool IsEndless(this StageDefinition stage)
        {
            return stage.GetProperty<bool>(ENDLESS);
        }
        public static bool IsEndless(this LevelEngine level)
        {
            return level.GetProperty<bool>(ENDLESS);
        }
    }
}
