using MukioI18n;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
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
    [StageDefinition(VanillaStageNames.triggerTutorial)]
    public class TriggerTutorialStage : StageDefinition
    {
        public TriggerTutorialStage(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            SetTutorialTimer(level, new FrameTimer(90));
            level.SetSeedSlotCount(1);
            level.FillSeedPacks(new NamespaceID[] { VanillaContraptionID.tnt });
            level.SetStarshardActive(false);
            level.SetTriggerActive(true);
            level.SetBlueprintsActive(true);
            level.SetPickaxeActive(false);
            level.SetEnergy(900);
            StartState(level, STATE_PLACE_TNT);
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
                case STATE_PLACE_TNT:
                    {
                        level.AddBuff<TutorialTriggerDisableBuff>();
                        var tntSeedPack = level.GetSeedPack(VanillaContraptionID.tnt);
                        level.SetHintArrowPointToBlueprint(tntSeedPack.GetIndex());
                        tntSeedPack.FullRecharge();
                    }
                    break;
                case STATE_CLICK_TRIGGER:
                    {
                        level.RemoveBuffs(level.GetBuffs<TutorialTriggerDisableBuff>());
                        var tntSeedPack = level.GetSeedPack(VanillaContraptionID.tnt);
                        tntSeedPack?.AddBuff<TutorialDisableBuff>();
                        level.SetHintArrowPointToTrigger();
                    }
                    break;
                case STATE_TRIGGER_TNT:
                    {
                        level.HideHintArrow();
                    }
                    break;
                case STATE_TNT_TRIGGERED:
                    {
                        level.AddBuff<TutorialTriggerDisableBuff>();
                        StartTimer(level, 150);
                    }
                    break;
                case STATE_CLICK_TRIGGER_SWAP:
                    {
                        level.RemoveBuffs(level.GetBuffs<TutorialTriggerDisableBuff>());
                        level.SetHintArrowPointToTrigger();
                    }
                    break;
                case STATE_CLICK_TNT_SWAP:
                    {
                        var tntSeedPack = level.GetSeedPack(VanillaContraptionID.tnt);
                        tntSeedPack.FullRecharge();
                        tntSeedPack.RemoveBuffs(tntSeedPack.GetBuffs<TutorialDisableBuff>());
                        level.SetHintArrowPointToBlueprint(tntSeedPack.GetIndex());
                    }
                    break;
                case STATE_PLACE_TNT_SWAP:
                    {
                        level.HideHintArrow();
                    }
                    break;
                case STATE_TNT_PLACED_SWAP:
                    {
                        StartTimer(level, 150);
                    }
                    break;

                case STATE_INSTANT_TRIGGER_1:
                case STATE_INSTANT_TRIGGER_2:
                case STATE_FINAL:
                    {
                        StartTimer(level, 150);
                    }
                    break;
            }
        }
        private void UpdateState(LevelEngine level)
        {
            var state = GetTutorialState(level);
            switch (state)
            {
                case STATE_PLACE_TNT:
                    {
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.tnt)
                        {
                            level.HideHintArrow();
                        }
                        if (level.EntityExists(VanillaContraptionID.tnt))
                        {
                            StartState(level, STATE_CLICK_TRIGGER);
                        }
                    }
                    break;
                case STATE_CLICK_TRIGGER:
                    {
                        var heldEntityType = level.GetHeldItemType();
                        if (heldEntityType == VanillaHeldTypes.trigger)
                        {
                            StartState(level, STATE_TRIGGER_TNT);
                        }
                    }
                    break;
                case STATE_TRIGGER_TNT:
                    {
                        if (level.EntityExists(e => e.GetDefinitionID() == VanillaContraptionID.tnt && TNT.IsIgnited(e)) || !level.EntityExists(VanillaContraptionID.tnt))
                        {
                            StartState(level, STATE_TNT_TRIGGERED);
                        }
                    }
                    break;
                case STATE_TNT_TRIGGERED:
                    {
                        RunTimer(level);
                    }
                    break;


                case STATE_CLICK_TRIGGER_SWAP:
                    {
                        var heldEntityType = level.GetHeldItemType();
                        if (heldEntityType == VanillaHeldTypes.trigger)
                        {
                            StartState(level, STATE_CLICK_TNT_SWAP);
                        }
                    }
                    break;
                case STATE_CLICK_TNT_SWAP:
                    {
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.tnt && level.GetHeldItemData().InstantTrigger)
                        {
                            // 下一状态
                            StartState(level, STATE_PLACE_TNT_SWAP);
                            break;
                        }
                        var heldEntityType = level.GetHeldItemType();
                        if (heldEntityType != VanillaHeldTypes.trigger)
                        {
                            // 返回之前的状态。
                            StartState(level, STATE_CLICK_TRIGGER_SWAP);
                        }
                    }
                    break;
                case STATE_PLACE_TNT_SWAP:
                    {
                        if (level.EntityExists(e => e.GetDefinitionID() == VanillaContraptionID.tnt && TNT.IsIgnited(e)))
                        {
                            // 下一状态
                            StartState(level, STATE_TNT_PLACED_SWAP);
                            break;
                        }
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID != VanillaContraptionID.tnt || !level.GetHeldItemData().InstantTrigger)
                        {
                            // 返回之前的状态。
                            StartState(level, STATE_CLICK_TRIGGER_SWAP);
                        }
                    }
                    break;
                case STATE_TNT_PLACED_SWAP:
                    {
                        RunTimer(level);
                    }
                    break;


                case STATE_INSTANT_TRIGGER_1:
                case STATE_INSTANT_TRIGGER_2:
                case STATE_FINAL:
                    {
                        RunTimer(level);
                    }
                    break;
            }
        }
        private void OnTimerStop(LevelEngine level)
        {
            var state = GetTutorialState(level);
            switch (state)
            {
                case STATE_TNT_TRIGGERED:
                    StartState(level, STATE_CLICK_TRIGGER_SWAP);
                    break;
                case STATE_TNT_PLACED_SWAP:
                    StartState(level, STATE_INSTANT_TRIGGER_1);
                    break;
                case STATE_INSTANT_TRIGGER_1:
                    StartState(level, STATE_INSTANT_TRIGGER_2);
                    break;
                case STATE_INSTANT_TRIGGER_2:
                    StartState(level, STATE_FINAL);
                    break;
                case STATE_FINAL:
                    level.StopLevel();
                    level.PlayMusic(VanillaMusicID.mainmenu);
                    level.HideAdvice();
                    level.SetEnergy(level.GetStartEnergy());
                    level.ClearSeedPacks();
                    level.ChangeStage(VanillaStageID.halloween7);
                    level.SetBlueprintsActive(true);
                    level.SetPickaxeActive(true);
                    level.SetStarshardActive(true);
                    level.SetTriggerActive(true);
                    Global.Game.Unlock(VanillaUnlockID.trigger);
                    level.SimpleStartTalk(VanillaTalkID.halloween7, 0, 2, onEnd: () => level.BeginLevel());
                    break;
            }
        }
        public static FrameTimer GetTutorialTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(ID, PROP_TUTORIAL_TIMER);
        public static void SetTutorialTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(ID, PROP_TUTORIAL_TIMER, value);
        public static int GetTutorialState(LevelEngine level) => level.GetBehaviourField<int>(ID, PROP_STATE);
        public static void SetTutorialState(LevelEngine level, int value) => level.SetBehaviourField(ID, PROP_STATE, value);
        private static readonly NamespaceID ID = VanillaStageID.triggerTutorial;

        public static readonly string[] tutorialStrings = new string[]
        {
            STRING_STATE_0,
            STRING_STATE_1,
            STRING_STATE_2,
            STRING_STATE_3,
            STRING_STATE_4,
            STRING_STATE_5,
            STRING_STATE_6,
            STRING_STATE_7,
            STRING_STATE_8,
            STRING_STATE_9,
            STRING_STATE_10
        };
        public const int STATE_PLACE_TNT = 0;
        public const int STATE_CLICK_TRIGGER = 1;
        public const int STATE_TRIGGER_TNT = 2;
        public const int STATE_TNT_TRIGGERED = 3;
        public const int STATE_CLICK_TRIGGER_SWAP = 4;
        public const int STATE_CLICK_TNT_SWAP = 5;
        public const int STATE_PLACE_TNT_SWAP = 6;
        public const int STATE_TNT_PLACED_SWAP = 7;
        public const int STATE_INSTANT_TRIGGER_1 = 8;
        public const int STATE_INSTANT_TRIGGER_2 = 9;
        public const int STATE_FINAL = 10;

        public const string CONTEXT_STATE_PREFIX = "advice.trigger_tutorial.";
        public const string CONTEXT_STATE = CONTEXT_STATE_PREFIX + "{0}";

        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "0")]
        public const string STRING_STATE_0 = "放置TNT！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "1")]
        public const string STRING_STATE_1 = "点击选中触发器！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "2")]
        public const string STRING_STATE_2 = "点击以触发TNT！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "3")]
        public const string STRING_STATE_3 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "4")]
        public const string STRING_STATE_4 = "选中触发器！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "5")]
        public const string STRING_STATE_5 = "点击TNT蓝图！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "6")]
        public const string STRING_STATE_6 = "放置TNT，它会立即被触发！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "7")]
        public const string STRING_STATE_7 = "干得好！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "8")]
        public const string STRING_STATE_8 = "立即触发只限于一次性消耗类型的器械。";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "9")]
        public const string STRING_STATE_9 = "如果你想交换这两个动作的效果，你可以在设置中更改。";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "10")]
        public const string STRING_STATE_10 = "祝你好运！";

        public static readonly VanillaLevelPropertyMeta PROP_STATE = new VanillaLevelPropertyMeta("state");
        public static readonly VanillaLevelPropertyMeta PROP_TUTORIAL_TIMER = new VanillaLevelPropertyMeta("tutorialTimer");
    }
}
