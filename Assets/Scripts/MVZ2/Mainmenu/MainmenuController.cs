using System;
using System.Collections;
using System.Collections.Generic;
using MukioI18n;
using MVZ2.GameContent;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuController : MonoBehaviour
    {
        public void Display()
        {
            gameObject.SetActive(true);
            foreach (var button in GetAllButtons())
            {
                button.Interactable = true;
            }
            ui.SetButtonActive(MainmenuButtonType.Almanac, false);
            ui.SetButtonActive(MainmenuButtonType.Store, false);
            ui.SetBackgroundDark(false);
            ui.SetRayblockerActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public void Init()
        {
            var userName = main.SaveManager.GetCurrentUserName();
            if (string.IsNullOrEmpty(userName))
            {
                ui.SetUserName(string.Empty);
                ShowInputNameDialog(false);
            }
            else
            {
                ui.SetUserName(userName);
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
        private void OnOptionsButtonClickCallback() { }
        private void OnHelpButtonClickCallback() { }
        private void OnUserManageButtonClickCallback() { }
        private void OnQuitButtonClickCallback()
        {
            Application.Quit();
        }

        private void OnAlmanacButtonClickCallback() { }
        private void OnStoreButtonClickCallback() { }
        private void OnMoreMenuButtonClickCallback() { }

        private void OnBackToMenuButtonClickCallback() { }
        private void OnArchiveButtonClickCallback() { }
        private void OnAchievementButtonClickCallback() { }

        private void OnInputNameConfirmCallback(string name)
        {
            if (!ValidateUserName(name, out var message))
            {
                var error = main.LanguageManager._(message);
                ui.SetInputNameDialogError(error);
                return;
            }
            main.SaveManager.SetCurrentUserName(name);
            main.SaveManager.SaveMetaList();
            ui.SetUserName(name);
            ui.SetInputNameDialogVisible(false);
        }
        private void OnInputNameCancelCallback()
        {
            if (!canCancelInputName)
            {
                var error = main.LanguageManager._(ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT);
                ui.SetInputNameDialogError(error);
                return;
            }
            ui.SetInputNameDialogVisible(false);
        }
        #endregion
        private IEnumerator StartAdventure()
        {
            ui.SetBackgroundDark(true);
            main.MusicManager.Stop();
            main.SoundManager.Play2D(SoundID.loseMusic);

            foreach (var button in GetAllButtons())
            {
                button.Interactable = false;
            }

            yield return new WaitForSeconds(6);
            var task = main.LevelManager.GotoLevelScene();
            while (!task.IsCompleted)
            {
                yield return null;
            }
            main.LevelManager.StartLevel(BuiltinAreaID.day, BuiltinStageID.prologue);
            Hide();
        }
        private IEnumerable<MainmenuButton> GetAllButtons()
        {
            return ui.GetAllButtons();
        }
        private void ShowInputNameDialog(bool canCancel)
        {
            ui.SetInputNameDialogVisible(true);
            canCancelInputName = canCancel;
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
        #region 属性字段
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT = "第一次游戏必须输入用户名";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_EMPTY = "用户名不能为空";
        [TranslateMsg("输入名称对话框的错误信息")]
        public const string ERROR_MESSAGE_NAME_DUPLICATE = "已经存在该用户名";
        private Dictionary<MainmenuButtonType, Action> mainmenuActionDict = new Dictionary<MainmenuButtonType, Action>();
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private MainmenuUI ui;
        private bool canCancelInputName;
        #endregion
    }
}
