using System;
using MVZ2.UI;

namespace MVZ2
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
