using MukioI18n;
using MVZ2.Level;
using MVZ2.UI;

namespace MVZ2
{
    using static MVZ2.UI.OptionsDialog;
    public class OptionsLogicLevel : OptionsLogic
    {
        public OptionsLogicLevel(OptionsDialog dialog, LevelController level) : base(dialog)
        {
            Level = level;
        }
        protected override void OnButtonClickCallback(ButtonType type)
        {
            base.OnButtonClickCallback(type);
            switch (type)
            {
                case ButtonType.LeaveLevel:
                    {
                        var title = Main.LanguageManager._(StringTable.BACK);
                        var desc = Main.LanguageManager._(DIALOG_DESC_LEAVE_LEVEL);
                        Main.Scene.ShowDialogConfirm(title, desc, async (confirm) =>
                        {
                            if (confirm)
                            {
                                Level.Dispose();
                                await Main.LevelManager.ExitLevelSceneAsync();
                                Main.Scene.DisplayPage(MainScenePageType.Mainmenu);
                            }
                        });
                    }
                    break;
                case ButtonType.Restart:
                    {
                        var title = Main.LanguageManager._(DIALOG_TITLE_RESTART);
                        var desc = Main.LanguageManager._(DIALOG_DESC_RESTART);
                        Main.Scene.ShowDialogConfirm(title, desc, async (confirm) =>
                        {
                            if (confirm)
                            {
                                Level.Dispose();
                                await Level.RestartLevel();
                            }
                        });
                    }
                    break;
            }
        }
        #region 更新元素
        protected override void UpdateAllElements()
        {
            base.UpdateAllElements();

            UpdateLeaveLevelButton();
            bool isInLevel = Level.IsGameStarted() && Level.GetCurrentFlag() <= 0;
            dialog.SetButtonActive(ButtonType.Difficulty, !isInLevel);
            dialog.SetButtonActive(ButtonType.Restart, isInLevel);

            dialog.SetButtonActive(ButtonType.MoreOptions, false);
            dialog.SetButtonActive(ButtonType.LeaveLevel, true);
        }
        protected void UpdateLeaveLevelButton()
        {
            var textKey = Main.SaveManager.IsPrologueCleared() ? OPTION_BACK_TO_MAP : OPTION_BACK_TO_MAINMENU;
            var text = Main.LanguageManager._(textKey);
            dialog.SetButtonText(TextButtonType.LeaveLevel, text);
        }
        #endregion
        [TranslateMsg("对话框标题")]
        public const string DIALOG_TITLE_RESTART = "重新开始";
        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_RESTART = "确认要重新开始关卡吗？\n本关的进度都将丢失。";
        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_LEAVE_LEVEL = "确认要返回吗？\n你的进度会被保存。";
        [TranslateMsg("选项")]
        public const string OPTION_BACK_TO_MAP = "返回地图";
        [TranslateMsg("选项")]
        public const string OPTION_BACK_TO_MAINMENU = "返回主菜单";

        public LevelController Level { get; private set; }
    }
}
