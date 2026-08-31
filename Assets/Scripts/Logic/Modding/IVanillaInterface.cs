#nullable enable

using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Modding
{
    public interface IVanillaInterface
    {
        // Stats  
        bool IsEnemyEncountered(IGlobalSaveData saves, NamespaceID enemyID);

        // 矿车桥接  
        bool HasActiveMinecart(LevelEngine level);
        bool IsMinecartEffectiveMouseMode(LevelEngine level);
        void SetMinecartMoveInput(LevelEngine level, int moveZ, int moveX);
    }
}
