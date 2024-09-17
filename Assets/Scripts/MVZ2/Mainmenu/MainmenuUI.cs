using System;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;

namespace MVZ2.Mainmenu
{
    public class MainmenuUI : MonoBehaviour
    {
        public void SetUserName(string name)
        {
            userNameText.text = name;
        }
        public void SetInputNameDialogVisible(bool visible)
        {
            SetInputNameDialogError(string.Empty);
            inputNameDialog.gameObject.SetActive(visible);
        }
        public void SetInputNameDialogError(string message)
        {
            inputNameDialog.SetErrorMessage(message);
        }
        private void Awake()
        {
            inputNameDialog.OnConfirm += OnInputNameConfirmCallback;
            inputNameDialog.OnCancel += OnInputNameCancelCallback;
        }
        private void OnInputNameConfirmCallback(string name)
        {
            OnInputNameConfirm?.Invoke(name);
        }
        private void OnInputNameCancelCallback()
        {
            OnInputNameCancel?.Invoke();
        }
        public event Action<string> OnInputNameConfirm;
        public event Action OnInputNameCancel;

        [SerializeField]
        private TextMeshPro userNameText;
        [SerializeField]
        private InputNameDialog inputNameDialog;
    }
}
