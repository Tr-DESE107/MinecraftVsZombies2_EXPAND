﻿using MukioI18n;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.starshardTutorial)]
    public class StarshardTutorialStage : StageDefinition
    {
        public StarshardTutorialStage(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            SetTutorialTimer(level, new FrameTimer(90));
            level.SetSeedSlotCount(0);
            level.FillSeedPacks(new NamespaceID[0]);
            level.SetStarshardCount(1);
            level.SetStarshardActive(true);
            level.SetBlueprintsActive(false);
            level.SetPickaxeActive(false);
            StartState(level, STATE_CLICK_STARSHARD);
        }
        public override void OnUpdate(LevelEngine level)
        {
            base.OnUpdate(level);
            UpdateState(level);
            level.CheckGameOver();
        }
        private void StartTimer(LevelEngine level, int timeout)
        {
            var timer = GetTutorialTimer(level);
            timer.ResetTime(timeout);
        }
        private void RunTimer(LevelEngine level)
        {
            var timer = GetTutorialTimer(level);
            timer.Run();
            if (timer.Expired)
            {
                OnTimerStop(level);
            }
        }
        private void StartState(LevelEngine level, int state)
        {
            SetTutorialState(level, state);
            var textKey = tutorialStrings[state];
            var context = string.Format(CONTEXT_STATE, state);
            level.ShowAdvice(context, textKey, 1000, -1);
            switch (state)
            {
                case STATE_CLICK_STARSHARD:
                    {
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.zombie);
                        level.SpawnEnemy(spawnDef, 2);
                        level.SetHintArrowPointToStarshard();
                    }
                    break;
                case STATE_EVOKE_DISPENSER:
                    level.HideHintArrow();
                    break;
                case STATE_DISPENSER_EVOKED:
                    StartTimer(level, 150);
                    break;
                case STATE_GREEN_ENEMY:
                    {
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.zombie);
                        var enemy = level.SpawnEnemy(spawnDef, 2);
                        enemy.AddBuff<StarshardCarrierBuff>();
                    }
                    break;
                case STATE_KILL_HELMET_ZOMBIE:
                    {
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.ironHelmettedZombie);
                        var enemy = level.SpawnEnemy(spawnDef, 2);
                    }
                    break;
            }
        }
        private void UpdateState(LevelEngine level)
        {
            var state = GetTutorialState(level);
            switch (state)
            {
                case STATE_CLICK_STARSHARD:
                    {
                        var heldEntityType = level.GetHeldItemType();
                        if (heldEntityType == VanillaHeldTypes.starshard)
                        {
                            StartState(level, STATE_EVOKE_DISPENSER);
                        }
                    }
                    break;
                case STATE_EVOKE_DISPENSER:
                    {
                        if (level.EntityExists(e => e.IsEvoked()))
                        {
                            StartState(level, STATE_DISPENSER_EVOKED);
                        }
                    }
                    break;
                case STATE_DISPENSER_EVOKED:
                    RunTimer(level);
                    break;
                case STATE_GREEN_ENEMY:
                    {
                        if (level.EntityExists(VanillaPickupID.starshard))
                        {
                            StartState(level, STATE_COLLECT_STARSHARD);
                        }
                    }
                    break;
                case STATE_COLLECT_STARSHARD:
                    {
                        foreach (var starshard in level.FindEntities(VanillaPickupID.starshard))
                        {
                            starshard.SetProperty(VanillaPickupProps.IMPORTANT, true);
                        }
                        if (level.GetStarshardCount() > 0)
                        {
                            StartState(level, STATE_KILL_HELMET_ZOMBIE);
                        }
                    }
                    break;
                case STATE_KILL_HELMET_ZOMBIE:
                    if (level.GetEntities(EntityTypes.ENEMY).Length <= 0)
                    {
                        foreach (var particle in level.FindEntities(VanillaEffectID.smoke))
                        {
                            particle.Remove();
                        }
                        level.StopLevel();
                        level.PlayMusic(VanillaMusicID.mainmenu);
                        level.HideAdvice();
                        level.SetEnergy(level.GetStartEnergy());
                        level.ClearSeedPacks();
                        level.ChangeStage(VanillaStageID.halloween2);
                        level.SetBlueprintsActive(true);
                        level.SetPickaxeActive(true);
                        level.SetStarshardActive(true);
                        Global.Game.Unlock(VanillaUnlockID.starshard);
                        level.SimpleStartTalk(VanillaTalkID.starshardTutorial, 1, 2, onEnd: () => level.BeginLevel());
                    }
                    break;
            }
        }
        private void OnTimerStop(LevelEngine level)
        {
            var state = GetTutorialState(level);
            switch (state)
            {
                case STATE_DISPENSER_EVOKED:
                    StartState(level, STATE_GREEN_ENEMY);
                    break;
            }
        }
        public static FrameTimer GetTutorialTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(ID, PROP_TUTORIAL_TIMER);
        public static void SetTutorialTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(ID, PROP_TUTORIAL_TIMER, value);
        public static int GetTutorialState(LevelEngine level) => level.GetBehaviourField<int>(ID, PROP_STATE);
        public static void SetTutorialState(LevelEngine level, int value) => level.SetBehaviourField(ID, PROP_STATE, value);
        private static readonly NamespaceID ID = VanillaStageID.starshardTutorial;

        public static readonly string[] tutorialStrings = new string[]
        {
            STRING_STATE_0,
            STRING_STATE_1,
            STRING_STATE_2,
            STRING_STATE_3,
            STRING_STATE_4,
            STRING_STATE_5,
        };
        public const int STATE_CLICK_STARSHARD = 0;
        public const int STATE_EVOKE_DISPENSER = 1;
        public const int STATE_DISPENSER_EVOKED = 2;
        public const int STATE_GREEN_ENEMY = 3;
        public const int STATE_COLLECT_STARSHARD = 4;
        public const int STATE_KILL_HELMET_ZOMBIE = 5;

        public const string CONTEXT_STATE_PREFIX = "advice.starshard_tutorial.";
        public const string CONTEXT_STATE = CONTEXT_STATE_PREFIX + "{0}";

        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "0")]
        public const string STRING_STATE_0 = "点击星之碎片槽选中星之碎片！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "1")]
        public const string STRING_STATE_1 = "点击发射器！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "2")]
        public const string STRING_STATE_2 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "3")]
        public const string STRING_STATE_3 = "绿色的怪物会携带星之碎片！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "4")]
        public const string STRING_STATE_4 = "点击收集星之碎片！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "5")]
        public const string STRING_STATE_5 = "现在干掉铁盔僵尸！";

        public static readonly VanillaLevelPropertyMeta<int> PROP_STATE = new VanillaLevelPropertyMeta<int>("state");
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_TUTORIAL_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("tutorialTimer");
    }
}
