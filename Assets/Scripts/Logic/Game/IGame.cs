using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Games
{
    public interface IGame : IGameContent, IGameLocalization, IGameTriggerSystem
    {
        bool IsInLevel();
        LevelEngine GetLevel();
        string DefaultNamespace { get; }
    }
}
