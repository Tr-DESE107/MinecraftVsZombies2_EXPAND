using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class DebugConsoleUI : MonoBehaviour
    {
        public bool IsCommandFocused()
        {
            return inputField.isFocused;
        }
        public void SetCommand(string command)
        {
            inputField.text = command;
            inputField.caretPosition = inputField.text.Length;
        }
        public string GetCommand() => inputField.text;
        public int GetCaretPosition() => inputField.caretPosition;
        public void SetCaretPosition(int caretPosition) => inputField.caretPosition = caretPosition;
        public void PrintLine(string text)
        {
            outputText.text += text + "\n";
            Canvas.ForceUpdateCanvases();
            outputScroll.verticalNormalizedPosition = 0;
        }
        public void ActivateInputField()
        {
            inputField.ActivateInputField();
            inputField.Select();
        }

        #region ×Ô¶¯²¹È«
        public void ShowAutoCompletePanel()
        {
            autoCompletePanel.SetActive(true);
        }
        public void HideAutoCompletePanel()
        {
            autoCompletePanel.SetActive(false);
        }
        public void SetAutoCompleteSelections(string[] autoComplete)
        {
            autoCompleteSelection.updateList(autoComplete.Length, (i, obj) =>
            {
                var item = obj.GetComponent<DebugConsoleAutoCompleteItem>();
                item.SetText(autoComplete[i]);
                item.SetIsOn(false);
            },
            (obj) =>
            {
                var item = obj.GetComponent<DebugConsoleAutoCompleteItem>();
                item.OnValueChanged += OnAutoCompleteItemValueChangedCallback;
            },
            (obj) =>
            {
                var item = obj.GetComponent<DebugConsoleAutoCompleteItem>();
                item.OnValueChanged -= OnAutoCompleteItemValueChangedCallback;
            });
        }
        public void SetCurrentAutoComplete(int index)
        {
            if (autoCompleteSelection != null && index >= 0)
            {
                autoCompleteScroll.verticalNormalizedPosition = index / (float)autoCompleteSelection.Count;
                autoCompleteSelection.getElement<DebugConsoleAutoCompleteItem>(index).SetIsOn(true);
            }
        }
        private void OnAutoCompleteItemValueChangedCallback(DebugConsoleAutoCompleteItem item, bool value)
        {
            OnAutoCompleteItemClick?.Invoke(autoCompleteSelection.indexOf(item));
        }
        #endregion

        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnCloseClick?.Invoke());
            submitButton.onClick.AddListener(() => OnSubmit?.Invoke(inputField.text));
            upButton.onClick.AddListener(() => OnArrowButtonClick?.Invoke(true));
            downButton.onClick.AddListener(() => OnArrowButtonClick?.Invoke(false));
            inputField.onSubmit.AddListener((text) => OnSubmit?.Invoke(text));
            inputField.onSelect.AddListener((text) => OnInputFieldFocus?.Invoke(true));
            inputField.onDeselect.AddListener((text) => OnInputFieldFocus?.Invoke(false));
            inputField.onValueChanged.AddListener((text) => OnInputFieldValueChanged?.Invoke(text));
        }
        public event Action OnCloseClick;
        public event Action<bool> OnArrowButtonClick;
        public event Action<string> OnSubmit;
        public event Action<string> OnInputFieldValueChanged;
        public event Action<bool> OnInputFieldFocus;
        public event Action<int> OnAutoCompleteItemClick;
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private Button submitButton;
        [SerializeField]
        private Button upButton;
        [SerializeField]
        private Button downButton;
        [SerializeField]
        private DebugConsoleInputField inputField;

        [Header("Output")]
        [SerializeField]
        private TextMeshProUGUI outputText;
        [SerializeField]
        private ScrollRect outputScroll;

        [Header("Auto Complete")]
        [SerializeField]
        private GameObject autoCompletePanel;
        [SerializeField]
        private ScrollRect autoCompleteScroll;
        [SerializeField]
        private ElementList autoCompleteSelection;
    }
}
