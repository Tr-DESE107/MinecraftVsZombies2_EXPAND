using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Games
{
    public interface IGame : IGameContent, IGameLocalization, IGameSaveData, IGameMetas, IGameTriggerSystem
    {
        bool IsInLevel();
        LevelEngine GetLevel();
        int GetGridLayerPriority(NamespaceID layer);
        string GetGridErrorMessage(NamespaceID error);
        string DefaultNamespace { get; }
    }
}
