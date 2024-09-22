using log4net.Core;
using MVZ2.Save;
using PVZEngine.Level;

namespace PVZEngine.Game
{
    public interface IGame : IContentProvider, ITranslator, ISaveDataProvider
    {
        bool IsInLevel();
        LevelEngine GetLevel();
    }
}
