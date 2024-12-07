using MukioI18n;
using MVZ2.GameContent.Stages;
using MVZ2.Level;
using MVZ2.UI;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;

namespace MVZ2.Options
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicLevel : OptionsLogic
    {
        public OptionsLogicLevel(OptionsDialog dialog, LevelController level) : base(dialog)
        {
            Level = level;
        }
        public override void InitDialog()
        {
            UpdateLeaveLevelButton();

            base.InitDialog();

            bool isInLevel = Level.IsGameStarted() || Level.GetCurrentFlag() > 0;
            dialog.SetButtonActive(ButtonType.Difficulty, !isInLevel);
            dialog.SetButtonActive(ButtonType.Restart, isInLevel);

            dialog.SetButtonActive(ButtonType.MoreOptions, false);
            dialog.SetButtonActive(ButtonType.LeaveLevel, true);
        }
        protected override async void OnButtonClickCallback(ButtonType type)
        {
            base.OnButtonClickCallback(type);
            switch (type)
            {
                case ButtonType.LeaveLevel:
                    {
                        if (Level.IsGameStarted() || Level.GetCurrentFlag() > 0)
                        {
                            var title = Main.LanguageManager._(VanillaStrings.BACK);
                            var desc = Main.LanguageManager._(DIALOG_DESC_LEAVE_LEVEL);

                            var result = await Main.Scene.ShowDialogSelectAsync(title, desc);
                            if (!result)
                                break;
                        }
                        if (Level.IsGameStarted())
                        {
                            Main.LevelManager.SaveLevel();
                        }
                        await Level.ExitLevel();
                    }
                    break;
                case ButtonType.Restart:
                    {
                        Level.ShowRestartConfirmDialog();
                    }
                    break;
                case ButtonType.Difficulty:
                    {
                        Level.UpdateDifficulty();
                    }
                    break;
            }
        }
        #region 更新元素
        protected void UpdateLeaveLevelButton()
        {
            var textKey = Main.SaveManager.IsLevelCleared(VanillaStageID.prologue) ? VanillaStrings.BACK_TO_MAP : VanillaStrings.BACK_TO_MAINMENU;
            var text = Main.LanguageManager._(textKey);
            dialog.SetButtonText(TextButtonType.LeaveLevel, text);
        }
        #endregion
        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_LEAVE_LEVEL = "确认要返回吗？\n你的进度会被保存。";

        public LevelController Level { get; private set; }
    }
}
