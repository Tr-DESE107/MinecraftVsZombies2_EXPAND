using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using MVZ2.UI;
using UnityEngine;

namespace MVZ2.Debugs
{
    public class DebugConsoleController : MonoBehaviour
    {
        public void Show()
        {
            LoadCommandHistory();
            gameObject.SetActive(true);
            ui.ActivateInputField();
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
        public void PrintLine(string text)
        {
            ui.PrintLine(text);
        }
        private void Awake()
        {
            ui.OnCloseClick += OnCloseCallback;
            ui.OnSubmit += OnSubmitCallback;
            ui.OnInputFieldFocus += OnInputFieldFocusCallback;
            ui.OnInputFieldValueChanged += OnInputFieldValueChangedCallback;
            ui.OnEndTextSelection += OnEndTextSelectionCallback;
            ui.OnAutoCompleteItemClick += OnAutoCompleteItemClickCallback;
        }
        private void Update()
        {
            HandleInputNavigation();
        }
        private void OnCloseCallback()
        {
            Hide();
        }
        private void OnSubmitCallback(string text)
        {
            ExecuteCommand(text);
        }
        private void OnInputFieldFocusCallback(bool focus)
        {
            UpdateInputFieldSuggestions();
        }
        private void OnInputFieldValueChangedCallback(string text)
        {
            UpdateInputFieldSuggestions();
        }
        private void OnEndTextSelectionCallback(string text, int start, int end)
        {
            UpdateInputFieldSuggestions();
        }
        private void OnAutoCompleteItemClickCallback(int index)
        {
            suggestionIndex = index;
            UpdateSelectedAutoComplete();
        }
        private void UpdateInputFieldSuggestions()
        {
            // 输入变化时更新自动补全
            string command = ui.GetCommand();
            if (ui.IsCommandFocused() && !string.IsNullOrWhiteSpace(command))
            {
                UpdateSuggestions(command, ui.GetCaretPosition());
            }
            else
            {
                HideAutoCompletePanel();
            }
        }
        void HandleInputNavigation()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (isAutoCompleteActive)
                {
                    NavigateSuggestions(-1);
                }
                else
                {
                    NavigateHistory(1);
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (isAutoCompleteActive)
                {
                    NavigateSuggestions(1);
                }
                else
                {
                    NavigateHistory(-1);
                }
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                AutoComplete();
                return;
            }
        }

        #region 自动补全
        private void NavigateSuggestions(int direction)
        {
            if (currentSuggestions.Count == 0)
                return;

            suggestionIndex = (suggestionIndex + direction + currentSuggestions.Count) % currentSuggestions.Count;
            UpdateSelectedAutoComplete();
        }

        void UpdateSuggestions(string text, int currentIndex)
        {
            string input = text.Substring(0, currentIndex);
            string[] parts = Main.DebugManager.SplitInputText(input);

            currentSuggestions.Clear();
            Main.DebugManager.FillSuggestions(parts, currentSuggestions);

            if (currentSuggestions.Count > 0)
            {
                suggestionIndex = 0;
                ShowAutoCompletePanel();
            }
            else
            {
                HideAutoCompletePanel();
            }
        }

        void ShowAutoCompletePanel()
        {
            ui.ShowAutoCompletePanel();
            isAutoCompleteActive = true;

            ui.SetAutoCompleteSelections(currentSuggestions.ToArray());
            UpdateSelectedAutoComplete();
        }
        private void HideAutoCompletePanel()
        {
            ui.HideAutoCompletePanel();
            isAutoCompleteActive = false;
        }
        private void UpdateSelectedAutoComplete()
        {
            ui.SetCurrentAutoComplete(suggestionIndex);
        }

        private void AutoComplete()
        {
            if (currentSuggestions.Count == 0 || suggestionIndex < 0)
                return;

            var fullText = ui.GetCommand();
            var caretIndex = ui.GetCaretPosition();
            var beforeText = fullText.Substring(0, caretIndex);
            var afterText = fullText.Substring(caretIndex);

            var splitedBefore = Main.DebugManager.SplitInputText(beforeText);
            var newBeforeText = string.Join(' ', splitedBefore.SkipLast(1));

            var completedText = currentSuggestions[suggestionIndex];
            if (!string.IsNullOrWhiteSpace(newBeforeText))
            {
                completedText = newBeforeText + " " + completedText;
            }
            ui.SetCommand(completedText + afterText);
            ui.SetCaretPosition(completedText.Length);
            HideAutoCompletePanel();
        }
        #endregion


        #region 历史

        void SaveCommandHistory()
        {
            if (!keepHistoryBetweenSessions)
                return;
            Main.DebugManager.SaveCommandHistory(commandHistory);
        }

        void LoadCommandHistory()
        {
            if (!keepHistoryBetweenSessions)
                return;

            commandHistory.Clear();
            Main.DebugManager.LoadCommandHistory(commandHistory);
        }
        private void AddToHistory(string command)
        {
            // 避免添加重复的连续命令
            if (commandHistory.Count == 0 || commandHistory.Last() != command)
            {
                commandHistory.Insert(0, command);
            }

            // 限制历史记录大小
            while (commandHistory.Count > maxHistorySize)
            {
                commandHistory.RemoveAt(commandHistory.Count - 1);
            }

            SaveCommandHistory();
        }
        private void NavigateHistory(int direction)
        {
            if (commandHistory.Count == 0)
                return;

            historyIndex = Mathf.Clamp(historyIndex + direction, -1, commandHistory.Count - 1);

            if (historyIndex == -1)
            {
                ui.SetCommand(string.Empty);
            }
            else
            {
                ui.SetCommand(commandHistory[historyIndex]);
            }
        }
        #endregion

        private void ExecuteCommand(string text)
        {
            string input = text.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return;

            AddToHistory(input);
            historyIndex = -1;
            ui.SetCommand(string.Empty);
            ui.ActivateInputField();
            HideAutoCompletePanel();

            Main.DebugManager.ExecuteCommand(input);
        }

        public MainManager Main => MainManager.Instance;
        private List<string> commandHistory = new List<string>();
        private int historyIndex;
        private List<string> currentSuggestions = new List<string>();
        private int suggestionIndex;
        private bool isAutoCompleteActive;
        [SerializeField]
        private bool keepHistoryBetweenSessions = true;
        [SerializeField]
        private int maxHistorySize = 100;
        [SerializeField]
        private DebugConsoleUI ui;
    }
}
