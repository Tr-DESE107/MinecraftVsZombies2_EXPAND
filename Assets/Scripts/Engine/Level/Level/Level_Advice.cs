using System;

namespace PVZEngine.LevelManagement
{
    public partial class Level
    {
        public void ShowAdvice(string text, int priority, int timeout)
        {
            if (AdvicePriority >= priority && AdviceTimeout == 0)
                return;
            AdvicePriority = priority;
            AdviceTimeout = timeout;
            OnShowAdvice?.Invoke(text);
        }
        public void HideAdvice()
        {
            AdvicePriority = 0;
            AdviceTimeout = 0;
            OnHideAdvice?.Invoke();
        }
        private void AdviceUpdate()
        {
            if (AdviceTimeout <= 0)
                return;
            AdviceTimeout--;
            if (AdviceTimeout > 0)
                return;
            HideAdvice();
        }
        public event Action<string> OnShowAdvice;
        public event Action OnHideAdvice;
        public int AdvicePriority { get; private set; }
        public int AdviceTimeout { get; private set; }
    }
}
