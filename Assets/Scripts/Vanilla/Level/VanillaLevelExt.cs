using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla.Level
{
    public static partial class VanillaLevelExt
    {
        public static void SpawnCarts(this LevelEngine game, NamespaceID cartRef, float x, float xInterval)
        {
            var carts = game.GetEntities(EntityTypes.CART);
            for (int i = 0; i < game.GetMaxLaneCount(); i++)
            {
                if (carts.Any(c => c.GetLane() == i && c.State == EntityStates.IDLE))
                    continue;
                Entity cart = game.Spawn(cartRef, new Vector3(x - i * xInterval, 0, game.GetEntityLaneZ(i)), null);
            }
        }
        public static void CheckGameOver(this LevelEngine level)
        {
            var gameOverEnemies = level.FindEntities(e => e.Position.x < GetBorderX(false) && e.IsAliveEnemy());
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
            if (level.GetHeldItemType() != BuiltinHeldTypes.blueprint)
                return null;
            var seed = level.GetSeedPackAt((int)level.GetHeldItemID());
            if (seed == null)
                return null;
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return null;
            return seedDef.GetSeedEntityID();
        }
        public static bool IsHoldingItem(this LevelEngine level)
        {
            var type = level.GetHeldItemType();
            return NamespaceID.IsValid(type) && type != BuiltinHeldTypes.none;
        }
        public static bool IsHoldingPickaxe(this LevelEngine level)
        {
            return level.GetHeldItemType() == VanillaHeldTypes.pickaxe;
        }
        public static bool IsHoldingStarshard(this LevelEngine level)
        {
            return level.GetHeldItemType() == VanillaHeldTypes.starshard;
        }
        public static bool IsHoldingBlueprint(this LevelEngine level, int i)
        {
            return level.GetHeldItemType() == BuiltinHeldTypes.blueprint && level.GetHeldItemID() == i;
        }
        public static void CreatePreviewEnemies(this LevelEngine level, IEnumerable<NamespaceID> validEnemies, Rect region)
        {
            List<NamespaceID> enemyIDToCreate = new List<NamespaceID>();
            foreach (var id in validEnemies)
            {
                var spawnDefinition = level.ContentProvider.GetSpawnDefinition(id);
                int count = spawnDefinition.GetPreviewCount();

                for (int i = 0; i < count; i++)
                {
                    enemyIDToCreate.Add(id);
                }
            }

            List<Entity> createdEnemies = new List<Entity>();
            float radius = 80;
            while (enemyIDToCreate.Count > 0)
            {
                var creatingEnemyId = enemyIDToCreate.ToArray();
                foreach (var id in creatingEnemyId)
                {
                    var x = Random.Range(region.xMin, region.xMax);
                    var z = Random.Range(region.yMin, region.yMax);
                    var y = level.GetGroundY(x, z);
                    Vector3 pos = new Vector3(x, y, z);

                    if (radius > 0 && createdEnemies.Any(e => Vector3.Distance(e.Position, pos) < radius))
                        continue;

                    Entity enm = level.Spawn(id, pos, null);
                    enm.SetPreviewEnemy(true);
                    createdEnemies.Add(enm);

                    enemyIDToCreate.Remove(id);
                }
                radius--;
            }
        }
        public static void RemovePreviewEnemies(this LevelEngine level)
        {
            foreach (var enemy in level.FindEntities(e => e.IsPreviewEnemy()))
            {
                enemy.Remove();
            }
        }

        #region Positions

        public static float GetLeftUIBorderX(this LevelEngine level)
        {
            if (Global.IsMobile())
            {
                return 160;
            }
            return GetBorderX(false);
        }
        public static Vector2 GetMoneyPanelEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + MONEY_PANEL_X_TO_LEFT;
            var y = MONEY_PANEL_Y_TO_BOTTOM;
            return new Vector2(x, y);
        }
        public static Vector2 GetStarshardEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + STARSHARD_X_TO_LEFT;
            var y = STARSHARD_Y_TO_BOTTOM;
            return new Vector2(x, y);
        }
        public static Vector2 GetEnergySlotEntityPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + ENERGY_SLOT_WIDTH * 0.5f;
            var y = GetScreenHeight() - ENERGY_SLOT_WIDTH * 0.5f;
            return new Vector2(x, y);
        }
        public static Vector2 GetScreenCenterPosition(this LevelEngine level)
        {
            var x = level.GetLeftUIBorderX() + SCREEN_WIDTH * 0.5f;
            var y = SCREEN_HEIGHT * 0.5f;
            return new Vector2(x, y);
        }
        public static Rect GetEnemySpawnRect()
        {
            return new Rect(MIN_PREVIEW_X, MIN_PREVIEW_Y, MAX_PREVIEW_X - MIN_PREVIEW_X, MAX_PREVIEW_Y - MIN_PREVIEW_Y);
        }
        public static float GetScreenHeight()
        {
            return SCREEN_HEIGHT;
        }
        public static float GetBorderX(bool right)
        {
            return right ? RIGHT_BORDER : LEFT_BORDER;
        }
        public static float GetAttackBorderX(bool right)
        {
            return right ? ATTACK_RIGHT_BORDER : ATTACK_LEFT_BORDER;
        }
        public static float GetPickupBorderX(bool right)
        {
            return right ? PICKUP_RIGHT_BORDER : PICKUP_LEFT_BORDER;
        }
        public static float GetEnemyRightBorderX()
        {
            return ENEMY_RIGHT_BORDER;
        }
        public const float ENERGY_SLOT_WIDTH = 48;

        public const float MONEY_PANEL_X_TO_LEFT = 16;
        public const float MONEY_PANEL_Y_TO_BOTTOM = 32;

        public const float STARSHARD_X_TO_LEFT = 128 + 165 + 16;
        public const float STARSHARD_Y_TO_BOTTOM = 32;

        public const float MIN_PREVIEW_X = 1080;
        public const float MAX_PREVIEW_X = 1300;
        public const float MIN_PREVIEW_Y = 50;
        public const float MAX_PREVIEW_Y = 450;
        public const float GRID_SIZE = 80;
        public const float LAWN_HEIGHT = 600;
        public const float LEVEL_WIDTH = 1400;
        public const float CART_START_X = 150;
        public const float CART_TARGET_X = LEFT_BORDER;
        public const float SCREEN_WIDTH = 800;
        public const float SCREEN_HEIGHT = 600;
        public const float LEFT_BORDER = 220;
        public const float RIGHT_BORDER = LEFT_BORDER + SCREEN_WIDTH;
        public const float PICKUP_LEFT_BORDER = LEFT_BORDER + 50;
        public const float PICKUP_RIGHT_BORDER = RIGHT_BORDER - 50;
        public const float ATTACK_LEFT_BORDER = LEFT_BORDER;
        public const float ATTACK_RIGHT_BORDER = RIGHT_BORDER;
        public const float ENEMY_RIGHT_BORDER = RIGHT_BORDER + 60;
        #endregion
    }
}
