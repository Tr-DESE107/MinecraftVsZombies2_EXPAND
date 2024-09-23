using MVZ2.Resources;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Games
{
    public interface IGame : IContentProvider, ITranslator, ISaveDataProvider, IMetaProvider
    {
        bool IsInLevel();
        LevelEngine GetLevel();
    }
}
