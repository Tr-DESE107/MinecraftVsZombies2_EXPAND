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
        public void Print(string text)
        {
            ui.Print(text);
        }
        private void Awake()
        {
            ui.OnCloseClick += OnCloseCallback;
            ui.OnArrowButtonClick += OnArrowButtonClickCallback;
            ui.OnSubmit += OnSubmitCallback;
            ui.OnInputFieldValueChanged += OnInputFieldValueChangedCallback;
            ui.OnAutoCompleteItemClick += OnAutoCompleteItemClickCallback;
            Application.logMessageReceivedThreaded += OnLogReceivedCallback;
        }
        private void Update()
        {
            HandleInputNavigation();
            if (CheckSuggestionDirty())
            {
                UpdateInputFieldSuggestions();
            }
            historyNavigated = false;
        }
        private void OnCloseCallback()
        {
            Hide();
        }
        private void OnArrowButtonClickCallback(bool up)
        {
            if (up)
            {
                NavigateHistory(1);
            }
            else
            {
                NavigateHistory(-1);
            }
        }
        private void OnSubmitCallback(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                AddToHistory(text);
                historyIndex = -1;
                Main.DebugManager.ExecuteCommand(text);
            }
            ui.SetCommand(string.Empty);
            ui.SetCaretPosition(0);
            HideAutoCompletePanel();
            ui.ActivateInputField();
        }
        private void OnInputFieldValueChangedCallback(string text)
        {
            UpdateInputFieldSuggestions();
        }
        private void OnAutoCompleteItemClickCallback(int index)
        {
            suggestionIndex = index;
            AutoComplete();
        }
        private void OnLogReceivedCallback(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                case LogType.Warning:
                    return;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    Print($"<color=red>{logString}</color>\n");
                    break;
            }
        }

        private bool CheckSuggestionDirty()
        {
            var input = ui.GetCommand();
            var caret = ui.GetCaretPosition();
            if (lastInput != input || lastCaret != caret)
            {
                lastInput = input;
                lastCaret = caret;
                return true;
            }
            return false;
        }
        private void UpdateInputFieldSuggestions()
        {
            // 输入变化时更新自动补全
            string command = ui.GetCommand();
            if (!historyNavigated && (command != null && command.StartsWith(DebugManager.COMMAND_CHARACTER)))
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
            if (Main.InputManager.GetKeyDownOrHold(KeyCode.UpArrow))
            {
                PressUp();
                return;
            }

            if (Main.InputManager.GetKeyDownOrHold(KeyCode.DownArrow))
            {
                PressDown();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                AutoComplete();
                return;
            }
        }
        void PressUp()
        {
            if (isAutoCompleteActive)
            {
                NavigateSuggestions(-1);
            }
            else
            {
                NavigateHistory(1);
            }
        }
        void PressDown()
        {
            if (isAutoCompleteActive)
            {
                NavigateSuggestions(1);
            }
            else
            {
                NavigateHistory(-1);
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
            if (parts.Length < 1)
                return;

            currentSuggestions.Clear();
            Main.DebugManager.FillSuggestions(parts, currentSuggestions);

            if (currentSuggestions.Count > 0)
            {
                suggestionIndex = 0;
                ShowAutoCompletePanel();
                ui.SetAutoCompletePosition(currentIndex - parts[parts.Length - 1].Length);
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
            completedText = DebugManager.COMMAND_CHARACTER + completedText;
            ui.SetCommand(completedText + afterText);
            ui.SetCaretPosition(completedText.Length);

            UpdateInputFieldSuggestions();
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

            var command = string.Empty;
            if (historyIndex >= 0)
            {
                command = commandHistory[historyIndex];
            }
            ui.SetCommand(command);
            ui.SetCaretPosition(command.Length);
            ui.ForceUpdateCommand();
            historyNavigated = true;
        }
        #endregion

        public MainManager Main => MainManager.Instance;
        private List<string> commandHistory = new List<string>();
        private int historyIndex;
        private List<string> currentSuggestions = new List<string>();
        private int suggestionIndex;
        private bool isAutoCompleteActive;

        private string lastInput;
        private int lastCaret = -1;
        private bool historyNavigated;
        [SerializeField]
        private bool keepHistoryBetweenSessions = true;
        [SerializeField]
        private int maxHistorySize = 100;
        [SerializeField]
        private DebugConsoleUI ui;
    }
}
