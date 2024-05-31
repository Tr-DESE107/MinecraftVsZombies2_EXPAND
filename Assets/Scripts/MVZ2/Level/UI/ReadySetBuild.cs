using System;

namespace MVZ2.Level.UI
{
    public class ReadySetBuild : LevelHintText
    {
        public void StartGame()
        {
            OnStartGameCalled?.Invoke();
        }
        public event Action OnStartGameCalled;
    }
}
