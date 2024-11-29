using MVZ2.GameContent.Contraptions;
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
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            switch (cmd)
            {
                case "create_seventh_slot_form":
                    ShowSeventhSlotDialog(system);
                    break;
                case "create_tutorial_form":
                    ShowTutorialDialog(system);
                    break;
                case "try_buy_seventh_slot":
                    TryBuySeventhSlot(system);
                    break;

                case "start_tutorial":
                    level.ChangeStage(VanillaStageID.tutorial);
                    break;
                case "prepare_starshard_tutorial":
                    {
                        var grid = level.GetGrid(4, 2);
                        if (grid == null)
                            break;
                        if (!grid.CanPlace(VanillaContraptionID.dispenser))
                            break;
                        var x = level.GetEntityColumnX(4);
                        var z = level.GetEntityLaneZ(2);
                        var y = level.GetGroundY(x, z);
                        var position = new Vector3(x, y, z);
                        level.Spawn(VanillaContraptionID.dispenser, position, null);
                    }
                    break;
                case "start_starshard_tutorial":
                    level.ChangeStage(VanillaStageID.starshard_tutorial);
                    break;
                case "start_trigger_tutorial":
                    level.ChangeStage(VanillaStageID.trigger_tutorial);
                    break;
            }
        }
        private void ShowSeventhSlotDialog(ITalkSystem system)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            if (level.StageID != VanillaStageID.halloween7)
            {
                Debug.LogError("尝试在非万圣夜场景创建第七卡槽对话框。");
                return;
            }

            var title = game.GetText(VanillaStrings.UI_PURCHASE);
            var desc = game.GetText(VanillaStrings.UI_CONFIRM_BUY_7TH_SLOT);
            var options = new string[]
            {
                game.GetText(VanillaStrings.UI_YES),
                game.GetText(VanillaStrings.UI_NO)
            };
            level.ShowDialog(title, desc, options, (index) =>
            {
                switch (index)
                {
                    case 0:
                        game.AddMoney(-750);
                        level.SetSeedSlotCount(7);
                        game.SetBlueprintSlots(7);
                        system.StartSection(3);
                        break;
                    case 1:
                        system.StartSection(4);
                        break;
                }
            });
        }
        private void ShowTutorialDialog(ITalkSystem system)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            if (level.StageID != VanillaStageID.prologue)
            {
                Debug.LogError("尝试在非教程场景创建教程对话框。");
                return;
            }

            var title = game.GetText(VanillaStrings.UI_TUTORIAL);
            var desc = game.GetText(VanillaStrings.UI_CONFIRM_TUTORIAL);
            var options = new string[]
            {
                game.GetText(VanillaStrings.UI_YES),
                game.GetText(VanillaStrings.UI_NO)
            };
            level.ShowDialog(title, desc, options, (index) =>
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
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            level.ShowMoney();
            if (game.GetMoney() >= 750)
            {
                system.StartSection(1);
            }
            else
            {
                system.StartSection(2);
            }
        }
    }
}
