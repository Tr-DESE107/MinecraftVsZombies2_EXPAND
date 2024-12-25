using System;
using MVZ2.Managers;
using MVZ2.Talk;
using MVZ2Logic.Talk;
using PVZEngine.Level;

namespace MVZ2.Talks
{
    public abstract class MVZ2TalkSystem : ITalkSystem
    {
        public MVZ2TalkSystem(TalkController talk)
        {
            this.talk = talk;
        }
        public void StartSection(int section)
        {
            talk.StartSection(section);
        }
        public abstract bool IsInArchive();
        public abstract bool IsInMap();
        public abstract LevelEngine GetLevel();

        public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect)
        {
            Main.Scene.ShowDialog(title, desc, options, onSelect);
        }
        protected MainManager Main => MainManager.Instance;
        protected TalkController talk;
    }
}
