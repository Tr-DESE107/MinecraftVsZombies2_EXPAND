using MukioI18n;
using MVZ2.Extensions;
using MVZ2.Level;
using MVZ2.Localization;
using PVZEngine;

namespace MVZ2.UI
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
                                Main.LevelManager.SaveLevel();
                                await Level.ExitLevel();
                            }
                        });
                    }
                    break;
                case ButtonType.Restart:
                    {
                        Level.ShowRestartConfirmDialog();
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
            var textKey = NamespaceID.IsValid(Main.SaveManager.GetLastMapID()) ? StringTable.BACK_TO_MAP : StringTable.BACK_TO_MAINMENU;
            var text = Main.LanguageManager._(textKey);
            dialog.SetButtonText(TextButtonType.LeaveLevel, text);
        }
        #endregion
        [TranslateMsg("对话框内容")]
        public const string DIALOG_DESC_LEAVE_LEVEL = "确认要返回吗？\n你的进度会被保存。";

        public LevelController Level { get; private set; }
    }
}
