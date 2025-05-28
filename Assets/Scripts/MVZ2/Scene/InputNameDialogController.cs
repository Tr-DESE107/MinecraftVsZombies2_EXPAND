﻿using System.Threading.Tasks;
using MVZ2.Managers;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Mainmenu.UI
{
    public class InputNameDialogController : MonoBehaviour
    {
        public Task<string> Show(InputNameType type)
        {
            ui.ResetPosition();
            ui.ClearContent();
            ui.SetErrorMessage(string.Empty);
            inputNameType = type;
            gameObject.SetActive(true);
            if (tcs != null)
                return tcs.Task;
            tcs = new TaskCompletionSource<string>();
            return tcs.Task;
        }
        public Task<string> ShowRename(int renameIndex)
        {
            var task = Show(InputNameType.Rename);
            renamingUserIndex = renameIndex;
            return task;
        }
        private void Hide()
        {
            gameObject.SetActive(false);
            inputNameType = InputNameType.None;
            renamingUserIndex = -1;
            if (tcs == null)
                return;
            tcs.TrySetResult(null);
            tcs = null;
        }
        private void Awake()
        {
            ui.OnConfirm += OnConfirmCallback;
            ui.OnCancel += OnCancelCallback;
        }
        private void OnConfirmCallback(string value)
        {
            if (!ValidateUserName(value, out var message))
            {
                var error = main.LanguageManager._(message);
                ui.SetErrorMessage(error);
                return;
            }
            tcs.TrySetResult(value);
            Hide();
        }
        private void OnCancelCallback()
        {
            if (inputNameType == InputNameType.Initialize)
            {
                var error = main.LanguageManager._(VanillaStrings.ERROR_MESSAGE_CANNOT_CANCEL_NAME_INPUT);
                ui.SetErrorMessage(error);
                return;
            }
            Hide();
        }
        private bool ValidateUserName(string name, out string message)
        {
            if (string.IsNullOrEmpty(name))
            {
                message = VanillaStrings.ERROR_MESSAGE_NAME_EMPTY;
                return false;
            }

            if (main.SaveManager.HasDuplicateUserName(name, renamingUserIndex))
            {
                message = VanillaStrings.ERROR_MESSAGE_NAME_DUPLICATE;
                return false;
            }

            if (inputNameType == InputNameType.Rename && !main.SaveManager.CanRenameUserTo(name))
            {
                message = VanillaStrings.ERROR_MESSAGE_CANNOT_USE_THIS_NAME;
                return false;
            }
            message = null;
            return true;
        }
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private InputNameDialog ui;
        private InputNameType inputNameType;
        private int renamingUserIndex = -1;
        private TaskCompletionSource<string> tcs;
    }
    public enum InputNameType
    {
        None,
        Initialize,
        CreateNewUser,
        Rename
    }
}
