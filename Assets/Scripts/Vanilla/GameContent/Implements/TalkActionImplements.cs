﻿using System.Collections;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Maps;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Archives;
using MVZ2Logic.Level;
using MVZ2Logic.Maps;
using MVZ2Logic.Modding;
using MVZ2Logic.Talk;
using PVZEngine.Callbacks;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Implements
{
    public class TalkActionImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(VanillaCallbacks.TALK_ACTION, TalkAction);
        }
        private void TalkAction(VanillaCallbacks.TalkActionParams param, CallbackResult result)
        {
            var system = param.system;
            var cmd = param.action;
            var parameters = param.parameters;
            TalkPreset preset = null;
            if (system.IsInArchive())
            {
                preset = new ArchivePreset(system.GetArchive());
            }
            else if (system.IsInMap())
            {
                preset = new MapPreset(system.GetMap());
            }
            else if (system.IsInLevel())
            {
                preset = new LevelPreset(system.GetLevel());
            }

            if (preset != null)
            {
                preset.TalkAction(system, cmd, parameters);
            }
        }
        private abstract class TalkPreset
        {
            public abstract void TalkAction(ITalkSystem system, string cmd, string[] parameters);
        }
        private class LevelPreset : TalkPreset
        {
            private LevelEngine level;
            public LevelPreset(LevelEngine level)
            {
                this.level = level;
            }
            public override void TalkAction(ITalkSystem system, string cmd, string[] parameters)
            {
                switch (cmd)
                {
                    case "create_tutorial_form":
                        ShowTutorialDialog(system);
                        break;
                    case "try_buy_seventh_slot":
                        TryBuySeventhSlot(system);
                        break;
                    case "create_seventh_slot_form":
                        ShowSeventhSlotDialog(system);
                        break;

                    case "start_tutorial":
                        level.ChangeStage(VanillaStageID.tutorial);
                        break;
                    case "prepare_starshard_tutorial":
                        {
                            var grid = level.GetGrid(4, 2);
                            if (grid == null)
                                break;
                            if (!grid.CanSpawnEntity(VanillaContraptionID.dispenser))
                                break;
                            var x = level.GetEntityColumnX(4);
                            var z = level.GetEntityLaneZ(2);
                            var y = level.GetGroundY(x, z);
                            var position = new Vector3(x, y, z);
                            level.Spawn(VanillaContraptionID.dispenser, position, null);
                        }
                        break;
                    case "start_starshard_tutorial":
                        level.ChangeStage(VanillaStageID.starshardTutorial);
                        break;
                    case "start_trigger_tutorial":
                        level.ChangeStage(VanillaStageID.triggerTutorial);
                        break;
                }
            }
            private void ShowTutorialDialog(ITalkSystem system)
            {
                var game = Global.Game;
                var title = game.GetText(VanillaStrings.UI_TUTORIAL);
                var desc = game.GetText(VanillaStrings.UI_CONFIRM_TUTORIAL);
                var options = new string[]
                {
                    game.GetText(VanillaStrings.YES),
                    game.GetText(VanillaStrings.NO)
                };
                system.ShowDialog(title, desc, options, (index) =>
                {
                    switch (index)
                    {
                        case 0:
                            system.StartSection(1);
                            break;
                        case 1:
                            system.StartSection(2);
                            break;
                    }
                });
            }
            private void TryBuySeventhSlot(ITalkSystem system)
            {
                var game = Global.Game;
                if (game.GetMoney() >= 750)
                {
                    level.ShowMoney();
                    level.SetMoneyFade(false);
                    system.StartSection(1);
                }
                else
                {
                    level.ShowMoney();
                    system.StartSection(2);
                }
            }
            private void ShowSeventhSlotDialog(ITalkSystem system)
            {
                var game = Global.Game;
                var title = game.GetText(VanillaStrings.UI_PURCHASE);
                var desc = game.GetText(VanillaStrings.UI_CONFIRM_BUY_7TH_SLOT);
                var options = new string[]
                {
                    game.GetText(VanillaStrings.YES),
                    game.GetText(VanillaStrings.NO)
                };


                system.ShowDialog(title, desc, options, (index) =>
                {
                    switch (index)
                    {
                        case 0:
                            game.AddMoney(-750);
                            game.Unlock(VanillaUnlockID.blueprintSlot1);
                            game.SaveToFile(); // 完成蓝图槽位交易后保存游戏。
                            level.UpdatePersistentLevelUnlocks();
                            system.StartSection(3);
                            level.SetMoneyFade(true);
                            break;
                        case 1:
                            system.StartSection(4);
                            level.SetMoneyFade(true);
                            break;
                    }
                });
            }
        }
        private class MapPreset : TalkPreset
        {
            private IMapInterface map;
            public MapPreset(IMapInterface map)
            {
                this.map = map;
            }
            public override void TalkAction(ITalkSystem system, string cmd, string[] parameters)
            {
                switch (cmd)
                {
                    case "goto_dream":
                        Global.Game.Unlock(VanillaUnlockID.enteredDream);
                        Global.Game.SetLastMapID(VanillaMapID.dream);
                        Global.Game.SaveToFile(); // 进入梦境过渡时保存游戏
                        Global.StartCoroutine(VanillaChapterTransitions.TransitionTalkToLevel(VanillaChapterTransitions.dream, VanillaAreaID.dream, VanillaStageID.dream1));
                        break;
                    case "show_nightmare":
                        map.SetPreset(VanillaMapPresetID.nightmare);
                        Global.Game.Unlock(VanillaUnlockID.dreamIsNightmare);
                        Global.Game.SaveToFile(); // 转换到噩梦世界时保存游戏
                        break;
                    case "goto_castle":
                        Global.Game.SetLastMapID(VanillaMapID.castle);
                        Global.Game.SaveToFile(); // 进入辉针城过渡时保存游戏
                        Global.StartCoroutine(VanillaChapterTransitions.TransitionTalkToLevel(VanillaChapterTransitions.castle, VanillaAreaID.castle, VanillaStageID.castle1));
                        break;
                    case "chapter_3_finish":
                        Global.StartCoroutine(VanillaChapterTransitions.TransitionEndToMap(VanillaChapterTransitions.castle, VanillaMapID.gensokyo));
                        break;
                    case "goto_mausoleum":
                        Global.Game.SetLastMapID(VanillaMapID.mausoleum);
                        Global.Game.SaveToFile(); // 进入大祀庙过渡时保存游戏
                        Global.StartCoroutine(VanillaChapterTransitions.TransitionTalkToLevel(VanillaChapterTransitions.mausoleum, VanillaAreaID.mausoleum, VanillaStageID.mausoleum1));
                        break;
                    case "chapter_4_finish":
                        IEnumerator coroutineFunc()
                        {
                            yield return VanillaChapterTransitions.TransitionEndToMap(VanillaChapterTransitions.mausoleum, VanillaMapID.gensokyo);
                            var title = Global.Game.GetText(VanillaStrings.UI_GAME_CLEARED);
                            var desc = Global.Game.GetText(VanillaStrings.UI_COMING_SOON);
                            var options = new string[] { Global.Game.GetText(VanillaStrings.CONFIRM) };
                            Global.ShowDialog(title, desc, options);
                        }
                        Global.StartCoroutine(coroutineFunc());
                        break;
                }
            }
        }
        private class ArchivePreset : TalkPreset
        {
            private IArchiveInterface archive;
            public ArchivePreset(IArchiveInterface archive)
            {
                this.archive = archive;
            }
            public override void TalkAction(ITalkSystem system, string cmd, string[] parameters)
            {
                switch (cmd)
                {
                    case "create_tutorial_form":
                        ShowTutorialDialog(system);
                        break;
                    case "try_buy_seventh_slot":
                        TryBuySeventhSlot(system);
                        break;
                    case "create_seventh_slot_form":
                        ShowSeventhSlotDialog(system);
                        break;
                    case "show_nightmare":
                        archive.SetBackground(VanillaArchiveBackgrounds.nightmare);
                        break;
                }
            }
            private void ShowTutorialDialog(ITalkSystem system)
            {
                var game = Global.Game;
                var title = game.GetTextParticular(VanillaStrings.ARCHIVE_BRANCH, VanillaStrings.CONTEXT_ARCHIVE);
                var desc = game.GetText(VanillaStrings.UI_CONFIRM_TUTORIAL);
                var options = new string[]
                {
                    game.GetText(VanillaStrings.YES),
                    game.GetText(VanillaStrings.NO)
                };
                system.ShowDialog(title, desc, options, (index) =>
                {
                    switch (index)
                    {
                        case 0:
                            system.StartSection(1);
                            break;
                        case 1:
                            system.StartSection(2);
                            break;
                    }
                });
            }
            private void TryBuySeventhSlot(ITalkSystem system)
            {
                var game = Global.Game;
                var title = game.GetTextParticular(VanillaStrings.ARCHIVE_BRANCH, VanillaStrings.CONTEXT_ARCHIVE);
                var desc = game.GetTextParticular(VanillaStrings.ARCHIVE_WHETHER_HAS_ENOUGH_MONEY, VanillaStrings.CONTEXT_ARCHIVE);
                var options = new string[]
                {
                    game.GetText(VanillaStrings.YES),
                    game.GetText(VanillaStrings.NO)
                };

                system.ShowDialog(title, desc, options, (index) =>
                {
                    switch (index)
                    {
                        case 0:
                            system.StartSection(1);
                            break;
                        case 1:
                            system.StartSection(2);
                            break;
                    }
                });
            }
            private void ShowSeventhSlotDialog(ITalkSystem system)
            {
                var game = Global.Game;
                var title = game.GetTextParticular(VanillaStrings.ARCHIVE_BRANCH, VanillaStrings.CONTEXT_ARCHIVE);
                var desc = game.GetText(VanillaStrings.UI_CONFIRM_BUY_7TH_SLOT);
                var options = new string[]
                {
                    game.GetText(VanillaStrings.YES),
                    game.GetText(VanillaStrings.NO)
                };


                system.ShowDialog(title, desc, options, (index) =>
                {
                    switch (index)
                    {
                        case 0:
                            system.StartSection(3);
                            break;
                        case 1:
                            system.StartSection(4);
                            break;
                    }
                });
            }
        }
    }
}
