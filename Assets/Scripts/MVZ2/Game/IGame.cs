using MVZ2.Save;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Games
{
    public interface IGame : IContentProvider, ITranslator, ISaveDataProvider
    {
        bool IsInLevel();
        LevelEngine GetLevel();
    }
}
