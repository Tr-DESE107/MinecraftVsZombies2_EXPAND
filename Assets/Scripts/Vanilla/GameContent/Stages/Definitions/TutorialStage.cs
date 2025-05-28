﻿using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.HeldItems;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Talk;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Stages
{
    [StageDefinition(VanillaStageNames.tutorial)]
    public class TutorialStage : StageDefinition
    {
        public TutorialStage(string nsp, string name) : base(nsp, name)
        {
        }
        public override void OnStart(LevelEngine level)
        {
            base.OnStart(level);
            SetTutorialTimer(level, new FrameTimer(90));
            SetTutorialRNG(level, level.CreateRNG());
            level.SetEnergy(150);
            level.SetSeedSlotCount(4);
            level.FillSeedPacks(new NamespaceID[]
            {
                VanillaContraptionID.dispenser,
                VanillaContraptionID.furnace,
                VanillaContraptionID.obsidian,
                VanillaContraptionID.mineTNT,
            });
            StartState(level, STATE_CLICK_DISPENSER);
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
        private int GetLaneWithoutDispensers(LevelEngine level)
        {
            var dispensers = level.FindEntities(VanillaContraptionID.dispenser);
            var lanes = new List<int>();
            int maxLane = level.GetMaxLaneCount();
            var tutorialRNG = GetTutorialRNG(level);
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
            SetTutorialState(level, state);
            var textKey = tutorialStrings[state];
            var context = string.Format(CONTEXT_STATE, state);
            level.ShowAdvice(context, textKey, 1000, -1);
            switch (state)
            {
                case STATE_CLICK_DISPENSER:
                    {
                        var dispenserSeedPack = level.GetSeedPack(VanillaContraptionID.dispenser);
                        level.GetSeedPack(VanillaContraptionID.furnace)?.AddBuff<TutorialDisableBuff>();
                        level.GetSeedPack(VanillaContraptionID.obsidian)?.AddBuff<TutorialDisableBuff>();
                        level.GetSeedPack(VanillaContraptionID.mineTNT)?.AddBuff<TutorialDisableBuff>();
                        level.AddBuff<TutorialPickaxeDisableBuff>();
                        level.SetNoEnergy(true);
                        dispenserSeedPack.SetTwinkling(true);
                        level.SetHintArrowPointToBlueprint(dispenserSeedPack.GetIndex());
                    }
                    break;
                case STATE_PLACE_DISPENSER:
                    level.GetSeedPack(VanillaContraptionID.dispenser)?.SetTwinkling(false);
                    level.HideHintArrow();
                    break;
                case STATE_DISPENSER_PLACED:
                    level.SetNoEnergy(false);
                    break;
                case STATE_COLLECT_REDSTONE:
                    {
                        var redstones = level.FindEntities(e => e.IsEntityOf(VanillaPickupID.redstone));
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
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.zombie);
                        var lane = GetLaneWithoutDispensers(level);
                        level.SpawnEnemy(spawnDef, lane);
                    }
                    break;
                case STATE_ZOMBIE_KILLED:
                case STATE_FURNACE_PLACED:
                case STATE_FURNACE_PLACED_2:
                case STATE_HELMET_ZOMBIE_BLOWEN_UP:
                    StartTimer(level, 90);
                    break;
                case STATE_PLACE_FURNACE:
                    var furnace = level.GetSeedPack(VanillaContraptionID.furnace);
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
                        var obsidian = level.GetSeedPack(VanillaContraptionID.obsidian);
                        if (obsidian != null)
                        {
                            var buffs = obsidian.GetBuffs<TutorialDisableBuff>();
                            obsidian.RemoveBuffs(buffs);
                            obsidian.SetTwinkling(true);
                            level.SetHintArrowPointToBlueprint(obsidian.GetIndex());
                        }

                        int maxLane = level.GetMaxLaneCount();
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.ironHelmettedZombie);
                        var dispensers = level.FindEntities(VanillaContraptionID.dispenser);
                        int lane;
                        if (dispensers.Count() <= 0)
                        {
                            var tutorialRNG = GetTutorialRNG(level);
                            lane = tutorialRNG.Next(0, maxLane);
                        }
                        else
                        {
                            lane = dispensers[0].GetLane();
                        }
                        var enemy = level.SpawnEnemy(spawnDef, lane);
                        var armor = enemy.GetMainArmor();
                        if (armor != null)
                        {
                            armor.Health = armor.GetMaxHealth() * 0.5f;
                        }
                    }
                    break;
                case STATE_HELMET_ZOMBIE_KILLED:
                    {
                        var obsidian = level.GetSeedPack(VanillaContraptionID.obsidian);
                        if (obsidian != null)
                        {
                            obsidian.SetTwinkling(false);
                        }
                        StartTimer(level, 90);
                    }
                    break;
                case STATE_CLICK_MINE_TNT:
                    {
                        var mineTNT = level.GetSeedPack(VanillaContraptionID.mineTNT);
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
                        level.GetSeedPack(VanillaContraptionID.mineTNT)?.SetTwinkling(false);
                        level.HideHintArrow();
                        var spawnDef = level.Content.GetSpawnDefinition(VanillaSpawnID.ironHelmettedZombie);
                        var lane = GetLaneWithoutDispensers(level);
                        level.SpawnEnemy(spawnDef, lane);
                    }
                    break;
                case STATE_HOLD_PICKAXE:
                    {
                        level.RemoveBuffs<TutorialPickaxeDisableBuff>();
                        level.SetNoEnergy(true);
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
            var state = GetTutorialState(level);
            switch (state)
            {
                case STATE_CLICK_DISPENSER:
                    {
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.dispenser)
                        {
                            StartState(level, STATE_PLACE_DISPENSER);
                        }
                    }
                    break;
                case STATE_PLACE_DISPENSER:
                    if (level.EntityExists(VanillaContraptionID.dispenser))
                    {
                        StartState(level, STATE_DISPENSER_PLACED);
                    }
                    break;
                case STATE_DISPENSER_PLACED:
                    if (level.EntityExists(VanillaPickupID.redstone))
                    {
                        StartState(level, STATE_COLLECT_REDSTONE);
                    }
                    break;
                case STATE_COLLECT_REDSTONE:
                    if (level.EntityExists(e => e.IsEntityOf(VanillaPickupID.redstone) && e.IsCollected()))
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
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.furnace)
                        {
                            level.GetSeedPack(VanillaContraptionID.furnace)?.SetTwinkling(false);
                            level.HideHintArrow();
                        }
                        if (level.EntityExists(VanillaContraptionID.furnace))
                        {
                            StartState(level, STATE_FURNACE_PLACED);
                        }
                    }
                    break;
                case STATE_PLACE_3_FURNACES:
                    if (level.FindEntities(VanillaContraptionID.furnace).Length >= 3 && level.Energy >= 50)
                    {
                        StartState(level, STATE_PLACE_OBSIDIAN);
                    }
                    break;
                case STATE_PLACE_OBSIDIAN:
                    {
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.obsidian)
                        {
                            level.GetSeedPack(VanillaContraptionID.obsidian)?.SetTwinkling(false);
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
                        var heldEntityID = level.GetHeldSeedEntityID();
                        if (heldEntityID == VanillaContraptionID.mineTNT)
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
                    if (level.GetHeldItemType() == VanillaHeldTypes.pickaxe)
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
                        foreach (var particle in level.FindEntities(VanillaEffectID.fragment))
                        {
                            particle.Remove();
                        }
                        level.StopLevel();
                        level.PlayMusic(VanillaMusicID.mainmenu);
                        level.HideAdvice();
                        level.SetEnergy(level.GetStartEnergy());
                        level.ResetAllRechargeProgress();
                        level.ClearSeedPacks();
                        level.SetNoEnergy(false);
                        level.ChangeStage(VanillaStageID.prologue);
                        for (int i = 0; i < level.GetSeedSlotCount(); i++)
                        {
                            var seedPack = level.GetSeedPackAt(i);
                            if (seedPack != null)
                            {
                                seedPack.SetTwinkling(false);
                            }
                        }
                        level.SimpleStartTalk(VanillaTalkID.tutorial, 3, 2, onEnd: () => level.BeginLevel());
                    }
                    break;
            }
        }
        private void OnTimerStop(LevelEngine level)
        {
            var state = GetTutorialState(level);
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

        public static FrameTimer GetTutorialTimer(LevelEngine level) => level.GetBehaviourField<FrameTimer>(ID, PROP_TUTORIAL_TIMER);
        public static void SetTutorialTimer(LevelEngine level, FrameTimer value) => level.SetBehaviourField(ID, PROP_TUTORIAL_TIMER, value);
        public static RandomGenerator GetTutorialRNG(LevelEngine level) => level.GetBehaviourField<RandomGenerator>(ID, PROP_TUTORIAL_RNG);
        public static void SetTutorialRNG(LevelEngine level, RandomGenerator value) => level.SetBehaviourField(ID, PROP_TUTORIAL_RNG, value);
        public static int GetTutorialState(LevelEngine level) => level.GetBehaviourField<int>(ID, PROP_STATE);
        public static void SetTutorialState(LevelEngine level, int value) => level.SetBehaviourField(ID, PROP_STATE, value);


        private static readonly NamespaceID ID = VanillaStageID.tutorial;
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
        public const string STRING_STATE_0 = "点击器械卡牌选中器械！";
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

        public static readonly VanillaLevelPropertyMeta<int> PROP_STATE = new VanillaLevelPropertyMeta<int>("state");
        public static readonly VanillaLevelPropertyMeta<RandomGenerator> PROP_TUTORIAL_RNG = new VanillaLevelPropertyMeta<RandomGenerator>("tutorialRNG");
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_TUTORIAL_TIMER = new VanillaLevelPropertyMeta<FrameTimer>("tutorialTimer");
    }
}
