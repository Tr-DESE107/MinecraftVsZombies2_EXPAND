#nullable enable

using System;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Obstacles;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Sprites;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Localization;
using MVZ2.Vanilla.Pickups;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Contents.Enemies;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Modifiers;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class PacZombieStageBehaviour : StageBehaviour
    {
        public PacZombieStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            // 借用星之碎片 UI 当命数显示：禁用星之碎片本身功能 + 换成命数图标  
            AddModifier(new NamespaceIDModifier(LogicLevelProps.STARSHARD_DISABLE_ID, SetOperator.Set, VanillaBlueprintErrors.locked));
            AddModifier(new SpriteReferenceModifier(LogicAreaProps.STARSHARD_ICON, SetOperator.Set, VanillaSprites.snipenserLife));
            stageDef.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostContraptionDeathCallback, filter: EntityTypes.PLANT);
        }

        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetPickaxeActive(false);
            level.SetTriggerActive(false);

            // 命数：初始 LIVES 条命  
            level.SetStarshardCount(LIVES);
            level.SetStarshardSlotCount(LIVES);

            RespawnDevourer(level);
        }

        public override void Update(LevelEngine level)
        {
            base.Update(level);
            if (level.CurrentWave >= 1)
            {
                var reference = GetDevourerReference(level);
                var devourer = reference?.GetEntity(level);
                if (!devourer.ExistsAndAlive())
                {
                    if (level.GetStarshardCount() > 0)
                    {
                        RespawnDevourer(level);
                        level.AddStarshardCount(-1);
                    }
                    else
                    {
                        level.GameOver(GameOverTypes.NO_ENEMY, null, VanillaStrings.DEATH_MESSAGE_SNIPENSER_LOST);
                    }
                }
            }
        }
        // —— 难度机制：第一面旗帜之后，每一大波（旗帜）到来时，  
        //    有概率在场上随机空格生成障碍物 gargoyleStatue / monsterSpawner。  
        //    仅本关（吃僵人）机制，写在本关行为里。  
        public override void PostWave(LevelEngine level, int wave)
        {
            base.PostWave(level, wave);

            // 第一面旗帜之后才开始（第 1 面旗帜前不生成，给玩家缓冲）  
            if (level.CurrentFlag < START_OBSTACLE_FLAG)
                return;

            var rng = GetOrCreateObstacleRNG(level);

            // 各自独立掷骰，命中才生成一批  
            if (rng.NextFloat() < STATUE_SPAWN_CHANCE)
                SpawnObstacle(level, rng, VanillaObstacleID.gargoyleStatue, STATUE_SPAWN_COUNT);

            if (rng.NextFloat() < SPAWNER_SPAWN_CHANCE)
                SpawnObstacle(level, rng, VanillaObstacleID.monsterSpawner, SPAWNER_SPAWN_COUNT);
        }

        // 通用：在随机空格生成指定障碍物 count 个（找不到空格自动少生成/不生成）  
        private void SpawnObstacle(LevelEngine level, RandomGenerator rng, NamespaceID obstacleID, int count)
        {
            var def = level.Content.GetEntityDefinition(obstacleID);
            if (def == null)
                return;

            // 障碍物占用的网格层，用于筛选“空的、能放下”的格子  
            var layersToTake = def.GetGridLayersToTake();
            // 从 OBSTACLE_MIN_COLUMN 列往右找空格，越靠右权重越高（不贴脸玩家起点）  
            var grids = level.FindObstacleSpawnGrids(layersToTake, rng, count, OBSTACLE_MIN_COLUMN, GetObstacleWeight);
            foreach (var grid in grids)
            {
                var pos = grid.GetEntityPosition();
                level.Spawn(obstacleID, pos, null)?.Let(e =>
                {
                    // 刷怪笼需要指定刷什么怪，否则不会刷怪  
                    if (e.IsEntityOf(VanillaObstacleID.monsterSpawner))
                    {
                        MonsterSpawner.SetEntityToSpawn(e, obstacleSpawnEnemies.Random(rng));
                        var param = e.GetSpawnParams();
                        param.SetProperty(LogicEntityProps.UPDATE_BEFORE_GAME, true);
                        e.Spawn(VanillaEffectID.spawnerAppearEmbers, e.GetCenter(), param);
                        e.PlaySound(VanillaSoundID.odd);
                    }
                });
            }
        }

        // 越靠右权重越大（与香草 Halloween/Mausoleum 一致）  
        private float GetObstacleWeight(LawnGrid grid) => grid.Column - OBSTACLE_MIN_COLUMN + 1;

        public static RandomGenerator GetOrCreateObstacleRNG(LevelEngine level)
        {
            var rng = level.GetProperty<RandomGenerator>(PROP_OBSTACLE_RNG);
            if (rng == null)
            {
                rng = level.CreateRNG();
                level.SetProperty(PROP_OBSTACLE_RNG, rng);
            }
            return rng;
        }

        private void RespawnDevourer(LevelEngine level)
        {
            var pos = new Vector3(level.GetEntityColumnX(2), 0, level.GetLaneZ(2));

            // 隐形矿车：已有就复用，没有再新建（复活时避免重复生成矿车）  
            var cart = SpawnOrFindMinecart(level, pos);
            if (cart == null)
                return;

            // 吃怪模式吞噬者，骑上矿车  
            var devParams = new SpawnParams();
            devParams.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());
            devParams.SetProperty(Devourer.PROP_ENDLESS_GHOST, true);
            var devourer = level.Spawn(VanillaContraptionID.devourer, cart.Position, cart, devParams);
            devourer? .AddBuff<DreamButterflyShieldBuff>();
            if (devourer == null)
                return;
            devourer.RideOn(cart);
            SetDevourerReference(level, new EntityID(devourer));
        }

        private Entity? SpawnOrFindMinecart(LevelEngine level, Vector3 pos)
        {
            // 需求1+3：优先复用已有矿车，并清除多余/残留矿车，保证唯一  
            var cart = MinecartRideable.FindSingleCart(level);
            if (cart != null)
                return cart;
            cart = level.Spawn(VanillaEffectID.minecartRideable, pos, null);
            if (cart != null)
                MinecartRideable.SetInvisible(cart);   // tint alpha=0 隐形  
            return cart;
        }

        private void PostContraptionDeathCallback(LevelCallbacks.EntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var level = entity.Level;
            if (level.HasBehaviour(this))
            {
                var snipenserReference = GetDevourerReference(level);
                if (snipenserReference != null && snipenserReference.IsEntity(entity))
                {

                    // ===== 新增：攻击力升级（HeavyWeaponAttackUp）红石返还 =====  
                    // 攻击力等级存在骑乘器械身上的 HeavyWeaponAttackUpBuff 里，取当前等级  
                    var attackUpBuff = entity.GetFirstBuff<HeavyWeaponAttackUpBuff>();
                    var attackUpLevel = attackUpBuff != null ? HeavyWeaponAttackUpBuff.GetLevel(attackUpBuff) : 0;
                    // 攻击力升级蓝图的花费，折算方式与射速/散射一致  
                    var attackUpCost = level.Content.GetSeedDefinition(VanillaBlueprintID.HeavyWeaponAttackUp)?.GetCost() ?? 0;
                    var attackUpRedstones = Mathf.Max(0, attackUpCost - 25) / 50;
                    // ===========================================================  

                    var totalRedstones = attackUpLevel * attackUpRedstones;

                    if (entity.GetFirstBuff<HeavyWeaponSelfDestructBuff>() != null)
                    {
                        totalRedstones *= 0.5f;
                    }

                    for (int i = 0; i < totalRedstones; i++)
                    {
                        entity.Produce(VanillaPickupID.redstone);
                    }

                    Explosion.Spawn(entity, entity.GetCenter(), 120);
                    entity.PlaySound(VanillaSoundID.largeExplosion);
                }
            }
        }

        public static EntityID? GetDevourerReference(LevelEngine level) => level.GetProperty<EntityID>(PROP_DEVOURER_REFERENCE);
        public static void SetDevourerReference(LevelEngine level, EntityID? value) => level.SetProperty(PROP_DEVOURER_REFERENCE, value);
        // 障碍物生成 RNG（存到关卡属性，保证存档/回放一致）  
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<RandomGenerator> PROP_OBSTACLE_RNG =
            new VanillaLevelPropertyMeta<RandomGenerator>("obstacle_rng");

        // ===== 可调参数 =====  
        public const int START_OBSTACLE_FLAG = 1;      // 第 1 面旗帜之后才开始生成  
        public const int OBSTACLE_MIN_COLUMN = 3;      // 从第几列往右才允许生成（避免贴脸）  
        public const float STATUE_SPAWN_CHANCE = 0.5f; // 每波生成石像鬼雕像的概率  
        public const float SPAWNER_SPAWN_CHANCE = 0.4f;// 每波生成刷怪笼的概率  
        public const int STATUE_SPAWN_COUNT = 1;       // 命中时生成雕像数量  
        public const int SPAWNER_SPAWN_COUNT = 1;      // 命中时生成刷怪笼数量  

        // 刷怪笼会刷出的敌人池（可按需增减）  
        public static readonly NamespaceID[] obstacleSpawnEnemies = new NamespaceID[]
        {
    VanillaEnemyID.zombie,
    VanillaEnemyID.leatherCappedZombie,
    VanillaEnemyID.ironHelmettedZombie,
        };
        public const int LIVES = 3;
        private const string PROP_REGION = "pac_zombie_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_DEVOURER_REFERENCE = new VanillaLevelPropertyMeta<EntityID>("devourer_reference");
    }
}
