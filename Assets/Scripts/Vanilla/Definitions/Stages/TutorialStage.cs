using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Buffs.SeedPack;
using MVZ2.GameContent.Contraptions;
using MVZ2.Level;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using static MVZ2.GameContent.BuffNames;

namespace MVZ2.GameContent.Stages
{
    public class TutorialStage : StageDefinition
    {
        public TutorialStage(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetProperty(PROP_TUTORIAL_TIMER, new FrameTimer(90));
            level.SetProperty(PROP_TUTORIAL_RNG, new RandomGenerator(level.Seed));
            level.SetEnergy(150);
            level.SetSeedPackCount(4);
            level.ReplaceSeedPacks(new NamespaceID[]
            {
                ContraptionID.dispenser,
                ContraptionID.furnace,
                ContraptionID.obsidian,
                ContraptionID.mineTNT,
            });
            StartState(level, STATE_CLICK_DISPENSER);
        }
        public override void Update(LevelEngine level)
        {
            base.Update(level);
            UpdateState(level);
        }
        private void StartTimer(LevelEngine level, int timeout)
        {
            var timer = level.GetProperty<FrameTimer>(PROP_TUTORIAL_TIMER);
            timer.MaxFrame = timeout;
            timer.Reset();
        }
        private void RunTimer(LevelEngine level)
        {
            var timer = level.GetProperty<FrameTimer>(PROP_TUTORIAL_TIMER);
            timer.Run();
            if (timer.Expired)
            {
                OnTimerStop(level);
            }
        }
        private int GetLaneWithoutDispensers(LevelEngine level)
        {
            var dispensers = level.FindEntities(ContraptionID.dispenser);
            var lanes = new List<int>();
            int maxLane = level.GetMaxLaneCount();
            var tutorialRNG = level.GetProperty<RandomGenerator>(PROP_TUTORIAL_RNG);
            for (int i = 0; i < maxLane; i++)
            {
                if (dispensers.All(d => d.GetLane() != i))
                {
                    lanes.Add(i);
                }
            }
            int lane;
            if (lanes.Count <= 0)
            {
                lane = tutorialRNG.Next(0, maxLane);
            }
            else
            {
                lane = lanes.Random(tutorialRNG);
            }
            return lane;
        }
        private void StartState(LevelEngine level, int state)
        {
            SetState(level, state);
            var textKey = tutorialStrings[state];
            var context = string.Format(CONTEXT_STATE, state);
            var advice = level.Translator.GetTextParticular(textKey, context);

            level.ShowAdvice(advice, 1000, -1);
            switch (state)
            {
                case STATE_CLICK_DISPENSER:
                    {
                        var dispenserSeedPack = level.GetSeedPack(ContraptionID.dispenser);
                        level.GetSeedPack(ContraptionID.furnace)?.AddBuff<TutorialDisableBuff>();
                        level.GetSeedPack(ContraptionID.obsidian)?.AddBuff<TutorialDisableBuff>();
                        level.GetSeedPack(ContraptionID.mineTNT)?.AddBuff<TutorialDisableBuff>();
                        level.AddBuff<TutorialPickaxeDisableBuff>();
                        level.SetNoProduction(true);
                        dispenserSeedPack.SetTwinkling(true);
                        level.SetHintArrowPointToBlueprint(dispenserSeedPack.GetIndex());
                    }
                    break;
                case STATE_PLACE_DISPENSER:
                    level.GetSeedPack(ContraptionID.dispenser)?.SetTwinkling(false);
                    level.HideHintArrow();
                    break;
                case STATE_DISPENSER_PLACED:
                    level.SetNoProduction(false);
                    break;
                case STATE_COLLECT_REDSTONE:
                    {
                        var redstones = level.FindEntities(e => e.Definition.GetID() == PickupID.redstone);
                        var redstone = redstones.FirstOrDefault();
                        if (redstone != null)
                        {
                            level.SetHintArrowPointToEntity(redstone);
                        }
                    }
                    break;
                case STATE_COLLECT_TO_PLACE_DISPENSER:
                    level.HideHintArrow();
                    break;
                case STATE_PLACE_DISPENSER_TO_KILL_ZOMBIE:
                    {
                        var spawnDef = level.ContentProvider.GetSpawnDefinition(EnemyID.zombie);
                        var lane = GetLaneWithoutDispensers(level);
                        level.SpawnEnemy(spawnDef, lane);
                    }
                    break;
                case STATE_ZOMBIE_KILLED:
                case STATE_FURNACE_PLACED:
                case STATE_FURNACE_PLACED_2:
                case STATE_HELMET_ZOMBIE_KILLED:
                case STATE_HELMET_ZOMBIE_BLOWEN_UP:
                    StartTimer(level, 90);
                    break;
                case STATE_PLACE_FURNACE:
                    var furnace = level.GetSeedPack(ContraptionID.furnace);
                    if (furnace != null)
                    {
                        var buffs = furnace.GetBuffs<TutorialDisableBuff>();
                        furnace.RemoveBuffs(buffs);
                        furnace.SetTwinkling(true);
                        level.SetHintArrowPointToBlueprint(furnace.GetIndex());
                    }
                    break;
                case STATE_PLACE_OBSIDIAN:
                    {
                        var obsidian = level.GetSeedPack(ContraptionID.obsidian);
                        if (obsidian != null)
                        {
                            var buffs = obsidian.GetBuffs<TutorialDisableBuff>();
                            obsidian.RemoveBuffs(buffs);
                            obsidian.SetTwinkling(true);
                            level.SetHintArrowPointToBlueprint(obsidian.GetIndex());
                        }

                        int maxLane = level.GetMaxLaneCount();
                        var spawnDef = level.ContentProvider.GetSpawnDefinition(EnemyID.ironHelmettedZombie);
                        var dispensers = level.FindEntities(ContraptionID.dispenser);
                        int lane;
                        if (dispensers.Count() <= 0)
                        {
                            var tutorialRNG = level.GetProperty<RandomGenerator>(PROP_TUTORIAL_RNG);
                            lane = tutorialRNG.Next(0, maxLane);
                        }
                        else
                        {
                            lane = dispensers[0].GetLane();
                        }
                        var enemy = level.SpawnEnemy(spawnDef, lane);
                        var armor = enemy.EquipedArmor;
                        if (armor != null)
                        {
                            armor.Health = armor.GetMaxHealth() * 0.5f;
                        }
                    }
                    break;
                case STATE_CLICK_MINE_TNT:
                    {
                        var mineTNT = level.GetSeedPack(ContraptionID.mineTNT);
                        if (mineTNT != null)
                        {
                            var buffs = mineTNT.GetBuffs<TutorialDisableBuff>();
                            mineTNT.RemoveBuffs(buffs);
                            mineTNT.SetTwinkling(true);
                            level.SetHintArrowPointToBlueprint(mineTNT.GetIndex());
                        }
                    }
                    break;
                case STATE_BLOWS_UP_HELMET_ZOMBIE:
                    {
                        level.GetSeedPack(ContraptionID.mineTNT)?.SetTwinkling(false);
                        level.HideHintArrow();
                        var spawnDef = level.ContentProvider.GetSpawnDefinition(EnemyID.ironHelmettedZombie);
                        var lane = GetLaneWithoutDispensers(level);
                        level.SpawnEnemy(spawnDef, lane);
                    }
                    break;
                case STATE_HOLD_PICKAXE:
                    {
                        var buffs = level.GetBuffs<TutorialPickaxeDisableBuff>();
                        level.RemoveBuffs(buffs);
                        level.SetNoProduction(true);
                        level.SetHintArrowPointToPickaxe();
                    }
                    break;
                case STATE_DIG_CONTRAPTIONS:
                    {
                        level.HideHintArrow();
                    }
                    break;
            }
        }
        private void UpdateState(LevelEngine level)
        {
            var state = GetState(level);
            switch (state)
            {
                case STATE_CLICK_DISPENSER:
                    {
                        var heldEntityID = level.GetHeldEntityID();
                        if (heldEntityID == ContraptionID.dispenser)
                        {
                            StartState(level, STATE_PLACE_DISPENSER);
                        }
                    }
                    break;
                case STATE_PLACE_DISPENSER:
                    if (level.EntityExists(ContraptionID.dispenser))
                    {
                        StartState(level, STATE_DISPENSER_PLACED);
                    }
                    break;
                case STATE_DISPENSER_PLACED:
                    if (level.EntityExists(PickupID.redstone))
                    {
                        StartState(level, STATE_COLLECT_REDSTONE);
                    }
                    break;
                case STATE_COLLECT_REDSTONE:
                    if (level.EntityExists(e => e.Definition.GetID() == PickupID.redstone && e.IsCollected()))
                    {
                        StartState(level, STATE_COLLECT_TO_PLACE_DISPENSER);
                    }
                    break;
                case STATE_COLLECT_TO_PLACE_DISPENSER:
                    if (level.Energy >= 100)
                    {
                        StartState(level, STATE_PLACE_DISPENSER_TO_KILL_ZOMBIE);
                    }
                    break;
                case STATE_PLACE_DISPENSER_TO_KILL_ZOMBIE:
                    if (level.GetEntities(EntityTypes.ENEMY).Length <= 0)
                    {
                        StartState(level, STATE_ZOMBIE_KILLED);
                    }
                    break;
                case STATE_ZOMBIE_KILLED:
                case STATE_FURNACE_PLACED:
                case STATE_FURNACE_PLACED_2:
                case STATE_HELMET_ZOMBIE_KILLED:
                case STATE_HELMET_ZOMBIE_BLOWEN_UP:
                    RunTimer(level);
                    break;
                case STATE_PLACE_FURNACE:
                    {
                        var heldEntityID = level.GetHeldEntityID();
                        if (heldEntityID == ContraptionID.furnace)
                        {
                            level.GetSeedPack(ContraptionID.furnace)?.SetTwinkling(false);
                            level.HideHintArrow();
                        }
                        if (level.EntityExists(ContraptionID.furnace))
                        {
                            StartState(level, STATE_FURNACE_PLACED);
                        }
                    }
                    break;
                case STATE_PLACE_3_FURNACES:
                    if (level.FindEntities(ContraptionID.furnace).Length >= 3 && level.Energy >= 50)
                    {
                        StartState(level, STATE_PLACE_OBSIDIAN);
                    }
                    break;
                case STATE_PLACE_OBSIDIAN:
                    {
                        var heldEntityID = level.GetHeldEntityID();
                        if (heldEntityID == ContraptionID.obsidian)
                        {
                            level.GetSeedPack(ContraptionID.obsidian)?.SetTwinkling(false);
                            level.HideHintArrow();
                        }
                        if (level.GetEntities(EntityTypes.ENEMY).Length <= 0)
                        {
                            StartState(level, STATE_HELMET_ZOMBIE_KILLED);
                        }
                    }
                    break;
                case STATE_CLICK_MINE_TNT:
                    {
                        var heldEntityID = level.GetHeldEntityID();
                        if (heldEntityID == ContraptionID.mineTNT)
                        {
                            StartState(level, STATE_BLOWS_UP_HELMET_ZOMBIE);
                        }
                    }
                    break;
                case STATE_BLOWS_UP_HELMET_ZOMBIE:
                    if (level.GetEntities(EntityTypes.ENEMY).Length <= 0)
                    {
                        StartState(level, STATE_HELMET_ZOMBIE_BLOWEN_UP);
                    }
                    break;
                case STATE_HOLD_PICKAXE:
                    foreach (var pickup in level.GetEntities(EntityTypes.PICKUP))
                    {
                        pickup.Timeout = Math.Min(pickup.Timeout, 30);
                    }
                    if (level.GetHeldItemType() == HeldTypes.pickaxe)
                    {
                        StartState(level, STATE_DIG_CONTRAPTIONS);
                    }
                    break;
                case STATE_DIG_CONTRAPTIONS:
                    foreach (var pickup in level.GetEntities(EntityTypes.PICKUP))
                    {
                        pickup.Timeout = Math.Min(pickup.Timeout, 30);
                    }
                    if (level.GetEntities(EntityTypes.PLANT).Length <= 0)
                    {
                        foreach (var particle in level.FindEntities(EffectID.fragment))
                        {
                            particle.Remove();
                        }
                        level.StopLevel();
                        level.PlayMusic(MusicID.mainmenu);
                        level.HideAdvice();
                        level.SetEnergy(level.Option.StartEnergy);
                        level.ClearSeedPacks();
                        level.SetNoProduction(false);
                        level.ChangeStage(StageID.prologue);
                        level.StartTalk(TalkID.tutorial, 3, 2);
                    }
                    break;
            }
        }
        private void OnTimerStop(LevelEngine level)
        {
            var state = GetState(level);
            switch (state)
            {
                case STATE_ZOMBIE_KILLED:
                    StartState(level, STATE_PLACE_FURNACE);
                    break;
                case STATE_FURNACE_PLACED:
                    StartState(level, STATE_FURNACE_PLACED_2);
                    break;
                case STATE_FURNACE_PLACED_2:
                    StartState(level, STATE_PLACE_3_FURNACES);
                    break;
                case STATE_HELMET_ZOMBIE_KILLED:
                    StartState(level, STATE_CLICK_MINE_TNT);
                    break;
                case STATE_HELMET_ZOMBIE_BLOWEN_UP:
                    StartState(level, STATE_HOLD_PICKAXE);
                    break;
            }
        }
        public void SetState(LevelEngine level, int value)
        {
            level.SetProperty(PROP_STATE, value);
        }
        public int GetState(LevelEngine level)
        {
            return level.GetProperty<int>(PROP_STATE);
        }

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
            STRING_STATE_10,
            STRING_STATE_11,
            STRING_STATE_12,
            STRING_STATE_13,
            STRING_STATE_14,
            STRING_STATE_15,
            STRING_STATE_16,
            STRING_STATE_17,
        };
        public const int STATE_CLICK_DISPENSER = 0;
        public const int STATE_PLACE_DISPENSER = 1;
        public const int STATE_DISPENSER_PLACED = 2;
        public const int STATE_COLLECT_REDSTONE = 3;
        public const int STATE_COLLECT_TO_PLACE_DISPENSER = 4;
        public const int STATE_PLACE_DISPENSER_TO_KILL_ZOMBIE = 5;
        public const int STATE_ZOMBIE_KILLED = 6;
        public const int STATE_PLACE_FURNACE = 7;
        public const int STATE_FURNACE_PLACED = 8;
        public const int STATE_FURNACE_PLACED_2 = 9;
        public const int STATE_PLACE_3_FURNACES = 10;
        public const int STATE_PLACE_OBSIDIAN = 11;
        public const int STATE_HELMET_ZOMBIE_KILLED = 12;
        public const int STATE_CLICK_MINE_TNT = 13;
        public const int STATE_BLOWS_UP_HELMET_ZOMBIE = 14;
        public const int STATE_HELMET_ZOMBIE_BLOWEN_UP = 15;
        public const int STATE_HOLD_PICKAXE = 16;
        public const int STATE_DIG_CONTRAPTIONS = 17;

        public const string CONTEXT_STATE_PREFIX = "advice.tutorial.";
        public const string CONTEXT_STATE = CONTEXT_STATE_PREFIX + "{0}";

        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "0")]
        public const string STRING_STATE_0 = "左键点击器械卡牌选中器械！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "1")]
        public const string STRING_STATE_1 = "点击草坪放置器械！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "2")]
        public const string STRING_STATE_2 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "3")]
        public const string STRING_STATE_3 = "点击红石收集能量！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "4")]
        public const string STRING_STATE_4 = "收集足够的能量来放置器械！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "5")]
        public const string STRING_STATE_5 = "用发射器干掉僵尸！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "6")]
        public const string STRING_STATE_6 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "7")]
        public const string STRING_STATE_7 = "选择熔炉并放置！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "8")]
        public const string STRING_STATE_8 = "熔炉能为你提供额外的红石！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "9")]
        public const string STRING_STATE_9 = "熔炉越多，你放器械的速度就越快！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "10")]
        public const string STRING_STATE_10 = "请至少放下三个熔炉！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "11")]
        public const string STRING_STATE_11 = "用黑曜石来挡住僵尸的进攻！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "12")]
        public const string STRING_STATE_12 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "13")]
        public const string STRING_STATE_13 = "地雷TNT在一段时间的填装后能炸飞敌人！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "14")]
        public const string STRING_STATE_14 = "用地雷TNT炸死铁盔僵尸！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "15")]
        public const string STRING_STATE_15 = "干得漂亮！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "16")]
        public const string STRING_STATE_16 = "最后，镐子能够挖掉你所放下的器械！";
        [TranslateMsg("教程关指引", CONTEXT_STATE_PREFIX + "17")]
        public const string STRING_STATE_17 = "试着挖掉所有器械！";

        public const string PROP_STATE = "state";
        public const string PROP_TUTORIAL_RNG = "tutorialRNG";
        public const string PROP_TUTORIAL_TIMER = "tutorialTimer";
    }
}
