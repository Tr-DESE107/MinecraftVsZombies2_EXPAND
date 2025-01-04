using System.Collections;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Maps;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Saves;
using MVZ2Logic;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using MVZ2Logic.Talk;
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
        private void TalkAction(ITalkSystem system, string cmd, string[] parameters)
        {
            TalkPreset preset = null;
            if (system.IsInArchive())
            {
                preset = new ArchivePreset();
            }
            else if (system.IsInMap())
            {
                preset = new MapPreset();
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
                            if (!grid.CanPlaceEntity(VanillaContraptionID.dispenser))
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
                            level.SetSeedSlotCount(game.GetBlueprintSlots());
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
            public override void TalkAction(ITalkSystem system, string cmd, string[] parameters)
            {
                switch (cmd)
                {
                    case "goto_dream":
                        Global.Game.Unlock(VanillaUnlockID.enteredDream);
                        Global.Game.SetLastMapID(VanillaMapID.dream);
                        Global.StartCoroutine(VanillaChapterTransitions.TransitionToLevel(VanillaChapterTransitions.dream, VanillaAreaID.dream, VanillaStageID.dream1));
                        break;
                }
            }
        }
        private class ArchivePreset : TalkPreset
        {
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
