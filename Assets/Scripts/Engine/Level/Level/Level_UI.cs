using System;

namespace PVZEngine.LevelManaging
{
    public partial class Level
    {
        public void BeginLevel(string transition)
        {
            OnBeginLevel?.Invoke(transition);
        }
        public void ShowMoney()
        {
            OnShowMoney?.Invoke();
        }
        public void ShowDialog(string title, string content, string[] options, Action<int> onConfirm = null)
        {
            OnShowDialog?.Invoke(title, content, options, onConfirm);
        }
        public event Action<string> OnBeginLevel;
        public event Action OnShowMoney;
        public event Action<string, string, string[], Action<int>> OnShowDialog;
    }
}
