using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGame : IGameContent, IGameLocalization, IGameTriggerSystem
    {
        string DefaultNamespace { get; }
    }
}
