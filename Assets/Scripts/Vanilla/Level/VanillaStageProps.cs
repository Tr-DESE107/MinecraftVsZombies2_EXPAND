﻿using System.Linq;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Level
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class VanillaStageProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<string> LEVEL_NAME = Get<string>("levelName");
        public static readonly PropertyMeta<int> DAY_NUMBER = Get<int>("dayNumber");

        public static readonly PropertyMeta<int> WAVE_MAX_TIME = Get<int>("waveMaxTime");
        public static readonly PropertyMeta<int> WAVE_ADVANCE_TIME = Get<int>("waveAdvanceTime");
        public static readonly PropertyMeta<float> WAVE_ADVANCE_HEALTH_PERCENT = Get<float>("waveAdvanceHealthPercent");

        public static readonly PropertyMeta<bool> NO_ENERGY = Get<bool>("noEnergy");
        public static readonly PropertyMeta<bool> AUTO_COLLECT_ALL = Get<bool>("autoCollectAll");
        public static readonly PropertyMeta<bool> AUTO_COLLECT_ENERGY = Get<bool>("autoCollectEnergy");
        public static readonly PropertyMeta<bool> AUTO_COLLECT_MONEY = Get<bool>("autoCollectMoney");
        public static readonly PropertyMeta<bool> AUTO_COLLECT_STARSHARD = Get<bool>("autoCollectStarshard");

        public static readonly PropertyMeta<bool> NO_START_TALK_MUSIC = Get<bool>("noStartTalkMusic");
        public static readonly PropertyMeta<IStageTalkMeta[]> TALKS = Get<IStageTalkMeta[]>("talks");

        public static readonly PropertyMeta<NamespaceID> CLEAR_PICKUP_MODEL = Get<NamespaceID>("clearPickupModel");
        public static readonly PropertyMeta<NamespaceID> CLEAR_PICKUP_BLUEPRINT = Get<NamespaceID>("clearPickupBlueprint");
        public static readonly PropertyMeta<bool> DROPS_TROPHY = Get<bool>("dropsTrophy");
        public static readonly PropertyMeta<NamespaceID> END_NOTE_ID = Get<NamespaceID>("endNoteId");

        public static readonly PropertyMeta<string> START_TRANSITION = Get<string>("startTransition");
        public static readonly PropertyMeta<int> START_CAMERA_POSITION = Get<int>("startCameraPosition");

        public static readonly PropertyMeta<bool> NEED_BLUEPRINTS = Get<bool>("needBlueprints");
        public static readonly PropertyMeta<NamespaceID> CLEAR_SOUND = Get<NamespaceID>("clearSound");

        public static readonly PropertyMeta<bool> ENDLESS = Get<bool>("endless");

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
        public static bool DropsTrophy(this LevelEngine level)
        {
            return level.GetProperty<bool>(DROPS_TROPHY);
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
        public static bool IsAutoCollectAll(this LevelEngine game)
        {
            return game.GetProperty<bool>(AUTO_COLLECT_ALL);
        }
        public static bool IsAutoCollectEnergy(this LevelEngine game)
        {
            return game.GetProperty<bool>(AUTO_COLLECT_ENERGY);
        }
        public static bool IsAutoCollectMoney(this LevelEngine game)
        {
            return game.GetProperty<bool>(AUTO_COLLECT_MONEY);
        }
        public static bool IsAutoCollectStarshard(this LevelEngine game)
        {
            return game.GetProperty<bool>(AUTO_COLLECT_STARSHARD);
        }
        public static bool IsNoEnergy(this LevelEngine game)
        {
            return game.GetProperty<bool>(NO_ENERGY);
        }
        public static void SetNoEnergy(this LevelEngine game, bool value)
        {
            game.SetProperty(NO_ENERGY, value);
        }
        public static bool NoStartTalkMusic(this LevelEngine game)
        {
            return game.GetProperty<bool>(NO_START_TALK_MUSIC);
        }
        public static IStageTalkMeta[] GetTalks(this LevelEngine game)
        {
            return game.GetProperty<IStageTalkMeta[]>(TALKS);
        }
        public static IStageTalkMeta[] GetTalksOfType(this LevelEngine game, string type)
        {
            var talks = game.GetTalks();
            if (talks == null)
                return null;
            return talks.Where(t => t.Type == type).ToArray();
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

        public static readonly PropertyMeta<bool> I_ZOMBIE = Get<bool>("iZombie");
        public static bool IsIZombie(this StageDefinition stage) => stage.GetProperty<bool>(I_ZOMBIE);
        public static bool IsIZombie(this LevelEngine level) => level.GetProperty<bool>(I_ZOMBIE);
        public static void SetIZombie(this StageDefinition stage, bool value) => stage.SetProperty(I_ZOMBIE, value);
    }
}
