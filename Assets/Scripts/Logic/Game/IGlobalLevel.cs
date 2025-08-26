using MVZ2.Logic.Level;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Games
{
    public interface IGlobalLevel
    {
        void InitLevel(NamespaceID areaId, NamespaceID stageId, float introDelay = 0, LevelExitTarget exitTarget = LevelExitTarget.MapOrMainmenu);
        LevelEngine GetLevel();
        public bool IsInLevel()
        {
            return GetLevel() != null;
        }
    }
}
