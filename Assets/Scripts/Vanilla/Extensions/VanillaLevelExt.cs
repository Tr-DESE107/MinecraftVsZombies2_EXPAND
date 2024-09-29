using System.Linq;
using MVZ2.Extensions;
using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public static class VanillaLevelExt
    {
        public static void CheckGameOver(this LevelEngine level)
        {
            var gameOverEnemies = level.FindEntities(e => e.Pos.x < BuiltinLevel.GetBorderX(false) && e.IsAliveEnemy());
            if (gameOverEnemies.Length > 0)
            {
                level.GameOver(GameOverTypes.ENEMY, gameOverEnemies.FirstOrDefault(), null);
            }
        }
        public static NamespaceID GetHeldEntityID(this LevelEngine level)
        {
            if (level.GetHeldItemType() != HeldTypes.blueprint)
                return null;
            var seed = level.GetSeedPackAt((int)level.GetHeldItemID());
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return null;
            return seedDef.GetSeedEntityID();
        }
    }
}
