using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.HeldItems;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.HeldItems;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Callbacks;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using Tools;
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
                if (carts.Any(c => c.GetLane() == i && c.State == VanillaEntityStates.IDLE))
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
        public static DamageOutput[] Explode(this LevelEngine level, Vector3 center, float radius, int faction, float amount, DamageEffectList effects, Entity source)
        {
            return level.Explode(center, radius, faction, amount, effects, new EntityReferenceChain(source));
        }
        public static DamageOutput[] Explode(this LevelEngine level, Vector3 center, float radius, int faction, float amount, DamageEffectList effects, EntityReferenceChain source)
        {
            List<DamageOutput> damageOutputs = new List<DamageOutput>();
            foreach (Entity entity in level.GetEntities())
            {
                if (!entity.IsHostile(faction))
                    continue;
                foreach (var collider in entity.GetCollidersInSphere(center, radius))
                {
                    var ent = collider.Entity;
                    var damageOutput = collider.TakeDamage(amount, effects, source);
                    if (damageOutput != null)
                    {
                        damageOutputs.Add(damageOutput);
                    }
                }
            }
            return damageOutputs.ToArray();
        }
        public static NamespaceID GetHeldEntityID(this LevelEngine level)
        {
            var heldType = level.GetHeldItemType();
            var heldDefinition = level.Content.GetHeldItemDefinition(heldType);
            if (heldDefinition == null)
                return null;
            var seed = heldDefinition.GetSeedPack(level, level.GetHeldItemData());
            var seedDef = seed?.Definition;
            if (seedDef == null)
                return null;
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
        public static bool IsHoldingConveyorBlueprint(this LevelEngine level, int i)
        {
            return level.GetHeldItemType() == BuiltinHeldTypes.conveyor && level.GetHeldItemID() == i;
        }
        public static bool IsHoldingTrigger(this LevelEngine level)
        {
            return level.GetHeldItemType() == VanillaHeldTypes.trigger;
        }
        public static void CreatePreviewEnemies(this LevelEngine level, IEnumerable<NamespaceID> validEnemies, Rect region)
        {
            List<NamespaceID> enemyIDToCreate = new List<NamespaceID>();
            foreach (var id in validEnemies)
            {
                var spawnDefinition = level.Content.GetSpawnDefinition(id);
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
        #region Waves
        public static void NextWave(this LevelEngine level)
        {
            if (level.IsHugeWave(level.CurrentWave))
            {
                level.CurrentFlag++;
            }
            level.CurrentWave++;
            var totalPoints = level.GetBaseSpawnPoints(level.CurrentWave);
            level.SetSpawnPoints(totalPoints);
            level.SpawnWaveEnemies(level.CurrentWave);

            var wave = level.CurrentWave;
            level.StageDefinition.PostWave(level, wave);
            level.Triggers.RunCallback(LevelCallbacks.POST_WAVE, level, wave);
        }
        public static float GetBaseSpawnPoints(this LevelEngine level, int wave)
        {
            var points = wave / 3f;
            if (level.IsHugeWave(wave))
            {
                points *= 2.5f;
            }
            points *= level.GetSpawnPointMultiplier();
            return Mathf.Ceil(points);
        }
        public static void SpawnWaveEnemies(this LevelEngine level, int wave)
        {
            var totalPoints = level.GetSpawnPoints();
            var pool = level.GetEnemyPool();
            var spawnDefs = pool.Where(e => e.CanSpawn(level)).Select(e => e.GetSpawnDefinition(level.Content));
            while (totalPoints > 0)
            {
                var validSpawnDefs = spawnDefs.Where(def => def.SpawnCost > 0 && def.SpawnCost <= totalPoints);
                if (validSpawnDefs.Count() <= 0)
                    break;
                var spawnDef = validSpawnDefs.Random(level.GetSpawnRNG());
                level.SpawnEnemyAtRandomLane(spawnDef);
                totalPoints -= spawnDef.SpawnCost;
            }

            if (level.IsFinalWave(wave))
            {
                var poolSpawnDefs = pool.Select(e => e.GetSpawnDefinition(level.Content));
                var notSpawnedDefs = poolSpawnDefs.Where(def => !level.IsEnemySpawned(def.GetID()));
                foreach (var notSpawnedDef in notSpawnedDefs)
                {
                    level.SpawnEnemyAtRandomLane(notSpawnedDef);
                }
            }
        }
        public static bool WillEnemySpawn(this LevelEngine level, NamespaceID spawnRef)
        {
            var pool = level.GetEnemyPool();
            if (pool == null)
                return false;
            return pool.Any(e => e.GetSpawnDefinition(level.Content).GetID() == spawnRef);
        }
        public static Entity SpawnEnemyAtRandomLane(this LevelEngine level, SpawnDefinition spawnDef)
        {
            if (spawnDef == null)
                return null;
            var lane = level.GetRandomEnemySpawnLane();
            return level.SpawnEnemy(spawnDef, lane);
        }
        public static Entity SpawnEnemy(this LevelEngine level, SpawnDefinition spawnDef, int lane)
        {
            if (spawnDef == null)
                return null;
            var x = level.GetEnemySpawnX();
            var z = level.GetEntityLaneZ(lane);
            var y = level.GetGroundY(x, z);
            var pos = new Vector3(x, y, z);
            var enemy = level.Spawn(spawnDef.EntityID, pos, null);
            level.AddSpawnedEnemyID(spawnDef.GetID());
            level.StageDefinition.PostEnemySpawned(enemy);
            level.Triggers.RunCallback(LevelCallbacks.POST_ENEMY_SPAWNED, enemy);
            return enemy;
        }
        #endregion

        #region 预览敌人
        public static void CreatePreviewEnemies(this LevelEngine level, Rect region)
        {
            var pool = level.GetEnemyPool();
            var validEnemies = pool.Select(e => e.GetSpawnDefinition(level.Content)?.EntityID);
            level.CreatePreviewEnemies(validEnemies, region);
        }
        public static void RemovePreviewEnemies(this LevelEngine level)
        {
            foreach (var enemy in level.FindEntities(e => e.IsPreviewEnemy()))
            {
                enemy.Remove();
            }
        }
        #endregion

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

        public const float ENEMY_LEFT_BORDER = LEFT_BORDER - 60;
        public const float ENEMY_RIGHT_BORDER = RIGHT_BORDER + 60;


        public const float PROJECTILE_LEFT_BORDER = LEFT_BORDER - 40;
        public const float PROJECTILE_RIGHT_BORDER = RIGHT_BORDER + 40;
        public const float PROJECTILE_UP_BORDER = 540;
        public const float PROJECTILE_DOWN_BORDER = -40;
        public const float PROJECTILE_TOP_BORDER = 1000;
        public const float PROJECTILE_BOTTOM_BORDER = -1000;
        #endregion

        public static SeedPack ConveyRandomSeedPack(this LevelEngine level)
        {
            if (level.CanConveySeedPack())
            {
                var id = level.DrawConveyorSeed();
                var seedPack = level.AddConveyorSeedPack(id ?? VanillaContraptionID.dispenser);
                if (seedPack != null)
                {
                    seedPack.SetDrawnConveyorSeed(seedPack.GetDefinitionID());
                }
                else
                {
                    level.PutSeedToConveyorPool(id);
                }
                return seedPack;
            }
            return null;
        }
        public static NamespaceID DrawConveyorSeed(this LevelEngine level)
        {
            var entries = level.GetConveyorPool();
            if (entries.Count() <= 0)
                return null;
            var index = level.GetConveyorRNG().WeightedRandom(entries.Select(e => Mathf.Max(1, e.Count - level.GetSpentSeedFromConveyorPool(e.ID))).ToArray());
            var entry = entries[index];
            level.SpendSeedFromConveyorPool(entry.ID);
            return entry.ID;
        }
        public static void Thunder(this LevelEngine level)
        {
            level.AddBuff<ThunderBuff>();
            foreach (var ghost in level.GetEntities())
            {
                foreach (var buff in ghost.GetBuffs<GhostBuff>())
                {
                    GhostBuff.Illuminate(buff);
                }
            }
            level.PlaySound(VanillaSoundID.thunder);
        }
    }
}
