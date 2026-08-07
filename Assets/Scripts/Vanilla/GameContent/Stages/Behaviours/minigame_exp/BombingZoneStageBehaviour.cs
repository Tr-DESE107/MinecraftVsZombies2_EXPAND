#nullable enable

using System;

using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Sprites;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Localization;
using MVZ2.Vanilla.Projectiles;
using MVZ2.Vanilla.Properties;

using MVZ2Logic;
using MVZ2Logic.Contents.Enemies;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Modifiers;

using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

using Tools;

using UnityEngine;

namespace MVZ2.GameContent.Stages
{
    public class BombingZoneStageBehaviour : StageBehaviour
    {
        public BombingZoneStageBehaviour(StageDefinition stageDef) : base(stageDef)
        {
            // 借用星之碎片 UI 当命数显示：禁用其原功能 + 换成命数图标    
            AddModifier(new NamespaceIDModifier(LogicLevelProps.STARSHARD_DISABLE_ID, SetOperator.Set, VanillaBlueprintErrors.locked));
            AddModifier(new SpriteReferenceModifier(LogicAreaProps.STARSHARD_ICON, SetOperator.Set, VanillaSprites.snipenserLife));
        }

        public override void Start(LevelEngine level)
        {
            base.Start(level);
            level.SetPickaxeActive(false);
            level.SetTriggerActive(false);

            // 命数    
            level.SetStarshardCount(LIVES);
            level.SetStarshardSlotCount(LIVES);

            SetDropTimer(level, new FrameTimer(DROP_INTERVAL));
            RespawnRider(level);
        }

        public override void Update(LevelEngine level)
        {
            base.Update(level);
            if (!level.IsGameRunning())
                return;

            // —— 复数生命：检测器械是否存活 ——    
            var riderRef = GetRiderReference(level);
            var rider = riderRef?.GetEntity(level);
            if (!rider.ExistsAndAlive())
            {
                if (level.GetStarshardCount() > 0)
                {
                    RespawnRider(level);
                    level.AddStarshardCount(-1);
                }
                else
                {
                    level.GameOver(GameOverTypes.NO_ENEMY, null, VanillaStrings.DEATH_MESSAGE_SNIPENSER_LOST);
                    return;
                }
            }

            // —— 落弹 ——    
            var timer = GetDropTimer(level);
            if (timer == null)
                return;
            timer.Run();
            if (timer.Expired)
            {
                timer.Reset();
                DropBomb(level);
                DropBomb(level);
                DropBomb(level);
            }
        }

        // 生成/复活：隐形矿车 + 骑乘器械，并记录实例引用    
        private void RespawnRider(LevelEngine level)
        {
            var cart = SpawnOrFindCart(level);
            if (cart == null)
                return;

            var param = new SpawnParams();
            param.SetProperty(LogicEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>());
            // 换成你想让玩家操纵的器械ID    
            var rider = level.Spawn(VanillaContraptionID.MegaSnipenser, cart.Position, cart, param);
            if (rider != null)
            {
                rider.RideOn(cart);
                SetRiderReference(level, new EntityID(rider));
            }
        }

        // 矿车死了通常还在；没有才新建，避免复活时多出一辆    
        private Entity? SpawnOrFindCart(LevelEngine level)
        {
            var cart = level.FindFirstEntity(VanillaEffectID.minecartRideable);
            if (cart.ExistsAndAlive())
                return cart;
            var pos = new Vector3(level.GetEntityColumnX(2), 0, level.GetLaneZ(2));
            cart = level.Spawn(VanillaEffectID.minecartRideable, pos, null);
            if (cart != null)
                MinecartRideable.SetInvisible(cart);
            return cart;
        }

        private void DropBomb(LevelEngine level)
        {
            var rng = level.GetSpawnRNG();

            // 随机落点（限制在场地可视范围内）    
            float x = BORDER_LEFT + rng.NextFloat() * (BORDER_RIGHT - BORDER_LEFT);
            int lane = rng.Next(level.GetMaxLaneCount());
            float z = level.GetEntityLaneZ(lane);
            float y = level.GetGroundY(x, z);
            var target = new Vector3(x, y, z);

            // 落点标识（加农炮指向标）    
            level.Spawn(VanillaEffectID.aimTarget, target, null)?.Let(m =>
            {
                m.Timeout = WARNING_FRAMES;
            });

            // 随机选炸弹 + 对应伤害（同一索引）    
            int index = rng.Next(BOMB_POOL.Length);
            var bombID = BOMB_POOL[index];
            float damage = BOMB_DAMAGE_POOL[index];

            var param = new SpawnParams();
            param.SetProperty(EngineEntityProps.FACTION, level.Option.RightFaction); // 敌方阵营    
            param.SetProperty(VanillaEntityProps.DAMAGE, damage);                    // 关键：赋予伤害    
            param.SetProperty(VanillaEntityProps.FALL_RESISTANCE, 100000f);          // 落地不因坠落受伤    

            // —— 加农炮导弹：模拟真实发射，让导弹用自身逻辑精确落到标识处 ——    
            if (bombID == VanillaProjectileID.cannonMissile)
            {
                // 落点与下落时间必须在 spawn 前写入：Init 会用 PROP_FALL_TIME 建计时器，  
                // 计时器到期后导弹会把自己瞬移到 PROP_TARGET_POSITION 上空垂直下落。  
                param.SetProperty(CannonMissile.PROP_TARGET_POSITION, target);
                param.SetProperty(CannonMissile.PROP_FALL_TIME, WARNING_FRAMES / 2);

                // 从落点正上方向上抛（和石砖加农炮 shotVelocity=(0,30,0) 一致），  
                // 不要再给它 lob 速度——否则会先飞一次再瞬移，造成落点偏移/二次下落。  
                var launchPos = new Vector3(x, y + LAUNCH_HEIGHT, z);
                level.Spawn(bombID, launchPos, null, param)?.Let(m =>
                {
                    m.Velocity = new Vector3(0, 30, 0);
                });
                return;
            }

            // —— 其它炸弹：高处抛落，用 lob 速度砸向落点 ——    
            var source = new Vector3(x, y + DROP_HEIGHT, z);
            level.Spawn(bombID, source, null, param)?.Let(bomb =>
            {
                bomb.Velocity = VanillaProjectileExt.GetLobVelocityByTime(
                    source, target, WARNING_FRAMES, bomb.GetGravity());

                if (bombID == VanillaContraptionID.tnt)
                {
                    IgnitableBehaviour.Ignite(bomb);
                    var t = IgnitableBehaviour.GetExplosionTimer(bomb);
                    t?.ResetTime(WARNING_FRAMES + 5);
                }
            });
        }

        // 炸弹池（已移除 explosiveLargeFireball——它按计时器爆炸，落点对不上标识）    
        private static readonly NamespaceID[] BOMB_POOL = new NamespaceID[]
        {
            VanillaContraptionID.tnt,             // 引燃TNT    
            VanillaEnemyID.MannequinTNT,          // 玩家模型TNT    
            VanillaEnemyID.PirateBomb,          // 玩家模型TNT    
            VanillaEnemyID.soulsand,
            VanillaProjectileID.missile,          // 科学怪人导弹    
            VanillaProjectileID.cannonMissile,    // 石砖加农炮导弹    
            VanillaProjectileID.beaconMeteor,     // 陨石    
            VanillaProjectileID.fireCharge,       // 火焰弹    
            VanillaProjectileID.boulder,          // 巨石    
        };

        // 伤害池：与 BOMB_POOL 一一对应（按手感自行调整）    
        private static readonly float[] BOMB_DAMAGE_POOL = new float[]
        {
            1800f,   // tnt    
            900f,   // MannequinTNT    
            900f,   // PirateBomb    
            0f,   // soulsand
            200f,   // missile    
            3600f,   // cannonMissile
            500f,   // beaconMeteor    
            150f,   // fireCharge    
            60f,   // boulder    
        };

        public static FrameTimer? GetDropTimer(LevelEngine level) => level.GetProperty<FrameTimer>(PROP_DROP_TIMER);
        public static void SetDropTimer(LevelEngine level, FrameTimer timer) => level.SetProperty(PROP_DROP_TIMER, timer);
        public static EntityID? GetRiderReference(LevelEngine level) => level.GetProperty<EntityID>(PROP_RIDER_REFERENCE);
        public static void SetRiderReference(LevelEngine level, EntityID? value) => level.SetProperty(PROP_RIDER_REFERENCE, value);

        public const int LIVES = 3;               // 器械命数    
        public const int DROP_INTERVAL = 45;      // 落弹间隔（帧）    
        public const int WARNING_FRAMES = 45;     // 预警时长/落体时间（帧）    
        public const float DROP_HEIGHT = 800f;    // 普通炸弹起始高度    
        public const float LAUNCH_HEIGHT = 200f;  // 加农炮导弹发射起点高度（避免出生即触地）    
        public const float BORDER_LEFT = 260f;    // 场地左右边界    
        public const float BORDER_RIGHT = 980f;

        private const string PROP_REGION = "bombing_zone_stage";
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<FrameTimer> PROP_DROP_TIMER =
            new VanillaLevelPropertyMeta<FrameTimer>("drop_timer");
        [LevelPropertyRegistry(PROP_REGION)]
        public static readonly VanillaLevelPropertyMeta<EntityID> PROP_RIDER_REFERENCE =
            new VanillaLevelPropertyMeta<EntityID>("rider_reference");
    }
}
