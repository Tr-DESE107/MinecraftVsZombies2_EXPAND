using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    public interface IPreviewStage
    {
        void CreatePreviewEnemies(LevelEngine level, Rect rect);
        void RemovePreviewEnemies(LevelEngine level);
    }
}
