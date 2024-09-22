using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MukioI18n;
using MVZ2.GameContent;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            foreach (var button in GetAllButtons())
            {
                button.Interactable = true;
            }
            ui.SetButtonActive(MainmenuButtonType.Almanac, false);
            ui.SetButtonActive(MainmenuButtonType.Store, false);
            ui.SetBackgroundDark(false);
            ui.SetOptionsDialogVisible(false);
            ui.SetInputNameDialogVisible(false);
            ui.SetUserManageDialogVisible(false);
            ui.SetRayblockerActive(true);

            if (!main.MusicManager.IsPlaying(MusicID.mainmenu))
            {
                main.MusicManager.Play(MusicID.mainmenu);
            }
            ui.SetUserName(main.SaveManager.GetCurrentUserName());
        }
        public void Reload()
        {
            Hide();
            Display();
        }
        public void Init()
        {
            var userName = main.SaveManager.GetCurrentUserName();
            if (string.IsNullOrEmpty(userName))
            {
                ShowInputNameDialog(InputNameType.Initialize);
            }
            ui.SetRayblockerActive(false);
        }
        #region 生命周期
        private void Awake()
        {
            mainmenuActionDict.Add(MainmenuButtonType.Adventure, OnAdventureButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Options, OnOptionsButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Help, OnHelpButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.UserManage, OnUserManageButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Quit, OnQuitButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Almanac, OnAlmanacButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Store, OnStoreButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.MoreMenu, OnMoreMenuButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.BackToMenu, OnBackToMenuButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Archive, OnArchiveButtonClickCallback);
            mainmenuActionDict.Add(MainmenuButtonType.Achievement, OnAchievementButtonClickCallback);
            ui.OnMainmenuButtonClick += OnMainmenuButtonClickCallback;

            ui.OnInputNameConfirm += OnInputNameConfirmCallback;
            ui.OnInputNameCancel += OnInputNameCancelCallback;

            ui.OnUserManageDialogButtonClick += OnUserManageButtonClickCallback;
            ui.OnUserManageDialogUserSelect += OnUserManageUserSelectCallback;
        }
        #endregion

        #region 事件回调
        private void OnMainmenuButtonClickCallback(MainmenuButtonType type)
        {
            if (mainmenuActionDict.TryGetValue(type, out var action))
            {
                action?.Invoke();
            }
        }
        private void OnAdventureButtonClickCallback()
        {
            StartCoroutine(StartAdventure());
        }
        private void OnOptionsButtonClickCallback()
        {
            ui.SetOptionsDialogVisible(true);
            optionsLogic = new OptionsLogicMainmenu(ui.OptionsDialog);
            optionsLogic.InitDialog();
            optionsLogic.OnClose += OnOptionsCloseClickCallback;
        }
        private void OnHelpButtonClickCallback()
        {
            main.SoundManager.Play2D(SoundID.paper);
            main.MusicManager.Stop();
            var buttonText = main.LanguageManager._(StringTable.BACK);
            main.Scene.DisplayNote(BuiltinNoteID.help, buttonText, () => main.Scene.DisplayPage(MainScenePageType.Mainmenu));
        }
        private void OnUserManageButtonClickCallback()
        {
            ui.SetUserManageDialogVisible(true);
            RefreshUserManageDialog();
        }
        private void OnQuitButtonClickCallback()
        {
            var title = main.LanguageManager._(StringTable.QUIT);
            var desc = main.LanguageManager._(QUIT_DESC);
            main.Scene.ShowDialogConfirm(title, desc, (value) =>
            {
                if (value)
                    Application.Quit();
            });
        }

        private void OnAlmanacButtonClickCallback() { }
        private void OnStoreButtonClickCallback() { }
        private void OnMoreMenuButtonClickCallback() { }

        private void OnBackToMenuButtonClickCallback() { }
        private void OnArchiveButtonClickCallback() { }
        private void OnAchievementButtonClickCallback() { }

        private void OnOptionsCloseClickCallback()
        {
            ui.SetOptionsDialogVisible(false);
            if (optionsLogic == null)
                return;
            if (optionsLogic.NeedsReload)
            {
                main.OptionsManager.SetLanguage(optionsLogic.Language);
                main.OptionsManager.SetBloodAndGore(optionsLogic.BloodAndGore);
                Reload();
            }
            optionsLogic.OnClose -= OnOptionsCloseClickCallback;
            optionsLogic.Dispose();
        }
        #region 输入用户名
        private void OnInputNameConfirmCallback(string name)
        {
            if (!ValidateUserName(name, out var message))
            {
                var error = main.LanguageManager._(message);
                ui.SetInputNameDialogError(error);
                return;
            }

            switch (inputNameType)
            {
                case InputNameType.Initialize:
                    RenameUser(main.SaveManager.GetCurrentUserIndex(), name);
                    break;
                case InputNameType.CreateNewUser:
                    main.SaveManager.CreateNewUser(name);
                    main.SaveManager.SaveUserList();
                    RefreshUserManageDialog();
                    break;
                case InputNameType.Rename:
                    RenameUser(renamingUserIndex, name);
                    break;
            }
            HideInputNameDialog();
        }
        private void OnInputNameCancelCallback()
        {
            if (inputNameType == InputNameType.Initialize)
            {
                var error = main.LanguageManager._(ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT);
                ui.SetInputNameDialogError(error);
                return;
            }
            HideInputNameDialog();
        }
        #endregion

        #region 用户管理
        private void OnUserManageUserSelectCallback(int index)
        {
            selectedUserArrayIndex = index;
            UpdateUserManageButtons();
        }
        private void OnUserManageButtonClickCallback(UserManageDialog.ButtonType type)
        {
            switch (type)
            {
                case UserManageDialog.ButtonType.CreateNewUser:
                    {
                        ShowInputNameDialog(InputNameType.CreateNewUser);
                    }
                    break;
                case UserManageDialog.ButtonType.Rename:
                    {
                        var userIndex = GetSelectedUserIndex();
                        renamingUserIndex = userIndex;
                        ShowInputNameDialog(InputNameType.Rename);
                    }
                    break;
                case UserManageDialog.ButtonType.Delete:
                    {
                        var userIndex = GetSelectedUserIndex();
                        var title = main.LanguageManager._(StringTable.WARNING);
                        var desc = main.LanguageManager._(WARNING_DELETE_USER, main.SaveManager.GetUserName(userIndex));
                        main.Scene.ShowDialogConfirm(title, desc, (value) =>
                        {
                            if (value)
                            {
                                DeleteUser(GetSelectedUserIndex());
                            }
                        });
                    }
                    break;
                case UserManageDialog.ButtonType.Switch:
                    {
                        var userIndex = GetSelectedUserIndex();
                        SwitchUser(userIndex);
                    }
                    break;
                case UserManageDialog.ButtonType.Back:
                    {
                        HideUserManageDialog();
                    }
                    break;
            }
        }
        #endregion

        #endregion
        private IEnumerator StartAdventure()
        {
            if (!main.IsFastMode())
            {
                ui.SetBackgroundDark(true);
                main.MusicManager.Stop();
                main.SoundManager.Play2D(SoundID.loseMusic);

                foreach (var button in GetAllButtons())
                {
                    button.Interactable = false;
                }

                yield return new WaitForSeconds(6);
            }
            var task = GotoLevel();
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
        private IEnumerable<MainmenuButton> GetAllButtons()
        {
            return ui.GetAllButtons();
        }
        private async Task GotoLevel()
        {
            await main.LevelManager.GotoLevelSceneAsync();
            main.LevelManager.StartLevel(BuiltinAreaID.day, BuiltinStageID.prologue);
            Hide();
        }
        #region 输入用户名
        private void ShowInputNameDialog(InputNameType type)
        {
            ui.SetInputNameDialogVisible(true);
            inputNameType = type;
        }
        private bool ValidateUserName(string name, out string message)
        {
            if (string.IsNullOrEmpty(name))
            {
                message = ERROR_MESSAGE_NAME_EMPTY;
                return false;
            }

            if (main.SaveManager.HasDuplicateUserName(name, main.SaveManager.GetCurrentUserIndex()))
            {
                message = ERROR_MESSAGE_NAME_DUPLICATE;
                return false;
            }
            message = null;
            return true;
        }
        private void HideInputNameDialog()
        {
            ui.SetInputNameDialogVisible(false);
            inputNameType = InputNameType.None;
            renamingUserIndex = -1;
        }
        #endregion

        #region 用户管理
        private void DeleteUser(int userIndex)
        {
            var currentUserIndex = main.SaveManager.GetCurrentUserIndex();
            var nextUserIndex = managingUserIndexes.FirstOrDefault(u => u != userIndex);
            main.SaveManager.DeleteUser(userIndex);
            if (userIndex == currentUserIndex)
            {
                main.SaveManager.SetCurrentUserIndex(nextUserIndex);
                HideUserManageDialog();
                Reload();
            }
            else
            {
                RefreshUserManageDialog();
            }
            main.SaveManager.SaveUserList();
        }
        private void RenameUser(int userIndex, string name)
        {
            main.SaveManager.SetUserName(userIndex, name);
            main.SaveManager.SaveUserList();
            RefreshUserManageDialog();
            var currentUserIndex = main.SaveManager.GetCurrentUserIndex();
            if (userIndex == currentUserIndex)
            {
                ui.SetUserName(name);
            }
        }
        private void SwitchUser(int userIndex)
        {
            main.SaveManager.SetCurrentUserIndex(userIndex);
            main.SaveManager.SaveUserList();
            HideUserManageDialog();
            Reload();
        }
        private int GetSelectedUserIndex()
        {
            return managingUserIndexes[selectedUserArrayIndex];
        }
        private void RefreshUserManageDialog()
        {
            var users = main.SaveManager.GetAllUsers();
            var currentIndex = main.SaveManager.GetCurrentUserIndex();
            var userIndexes = users.Select((user, index) => (user, index)).Where(a => a.user != null).Select(p => p.index);
            var reorderedUserPairs = userIndexes.Where(i => i != currentIndex).Prepend(currentIndex);
            managingUserIndexes = reorderedUserPairs.ToArray();
            selectedUserArrayIndex = Array.IndexOf(managingUserIndexes, currentIndex);
            var names = new string[managingUserIndexes.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = main.SaveManager.GetUserName(managingUserIndexes[i]);
            }
            ui.UpdateUserManageDialog(names, selectedUserArrayIndex);
            UpdateUserManageButtons();
        }
        private void HideUserManageDialog()
        {
            selectedUserArrayIndex = -1;
            managingUserIndexes = null;
            ui.SetUserManageDialogVisible(false);
        }
        private void UpdateUserManageButtons()
        {
            bool selected = selectedUserArrayIndex >= 0;
            ui.SetUserManageCreateNewUserActive(managingUserIndexes.Length < SaveManager.MAX_USER_COUNT);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Rename, selected);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Delete, selected && managingUserIndexes.Length >= 2);
            ui.SetUserManageButtonInteractable(UserManageDialog.ButtonType.Switch, selected && GetSelectedUserIndex() != main.SaveManager.GetCurrentUserIndex());
        }
        #endregion

        #region 属性字段
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT = "第一次游戏必须输入用户名";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_EMPTY = "用户名不能为空";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_DUPLICATE = "已经存在该用户名";
        [TranslateMsg("删除用户时的警告，{0}为名称")]
        public const string WARNING_DELETE_USER = "确认删除用户{0}吗？\n该用户所有的数据都将被删除！";
        [TranslateMsg("退出对话框的描述")]
        public const string QUIT_DESC = "确认要退出吗？";

        private Dictionary<MainmenuButtonType, Action> mainmenuActionDict = new Dictionary<MainmenuButtonType, Action>();
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private MainmenuUI ui;

        private OptionsLogicMainmenu optionsLogic;
        private int[] managingUserIndexes;
        private int selectedUserArrayIndex = -1;
        private int renamingUserIndex = -1;
        private InputNameType inputNameType;
        #endregion

        #region 内嵌类
        public enum InputNameType
        {
            None,
            Initialize,
            CreateNewUser,
            Rename
        }
        #endregion
    }
}
