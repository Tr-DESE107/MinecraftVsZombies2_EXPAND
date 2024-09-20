using System;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu
{
    public class InputNameDialog : Dialog
    {
        public void SetErrorMessage(string error)
        {
            errorMessage.text = error;
        }
        public void ClearContent()
        {
            inputField.text = string.Empty;
        }
        private void Awake()
        {
            confirmButton.onClick.AddListener(() => OnConfirm?.Invoke(inputField.text));
            cancelButton.onClick.AddListener(() => OnCancel?.Invoke());
        }
        public event Action<string> OnConfirm;
        public event Action OnCancel;
        [SerializeField]
        private TextMeshProUGUI errorMessage;
        [SerializeField]
        private TMP_InputField inputField;
        [SerializeField]
        private Button confirmButton;
        [SerializeField]
        private Button cancelButton;
    }
}
