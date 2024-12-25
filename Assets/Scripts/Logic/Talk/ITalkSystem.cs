using MVZ2Logic.Scenes;
using PVZEngine.Level;

namespace MVZ2Logic.Talk
{
    public interface ITalkSystem : IDialogDisplayer
    {
        void StartSection(int section);
        bool IsInArchive();
        bool IsInMap();
        bool IsInLevel() => GetLevel() != null;
        LevelEngine GetLevel();
    }

}
