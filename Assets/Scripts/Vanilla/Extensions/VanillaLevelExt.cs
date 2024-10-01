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
            var gameOverEnemies = level.FindEntities(e => e.Position.x < BuiltinLevel.GetBorderX(false) && e.IsAliveEnemy());
            if (gameOverEnemies.Length > 0)
            {
                level.GameOver(GameOverTypes.ENEMY, gameOverEnemies.FirstOrDefault(), null);
            }
        }
        public static void Explode(this LevelEngine level, Vector3 center, float radius, int faction, float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            foreach (Entity entity in level.GetEntities())
            {
                if (entity.IsEnemy(faction) && Detection.IsInSphere(entity, center, radius))
                {
                    entity.TakeDamage(amount, effects, source);
                }
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
