using MVZ2Logic.Saves;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Games
{
    public interface IGame : IContentProvider, ITranslator, ISaveDataProvider, IMetaProvider
    {
        bool IsInLevel();
        LevelEngine GetLevel();
    }
}
