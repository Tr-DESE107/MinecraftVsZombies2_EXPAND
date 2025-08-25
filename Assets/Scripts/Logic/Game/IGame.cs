using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Games
{
    public interface IGame : IGameContent, IGameLocalization, IGameSaveData, IGameTriggerSystem
    {
        bool IsInLevel();
        LevelEngine GetLevel();
        string DefaultNamespace { get; }
    }
}
