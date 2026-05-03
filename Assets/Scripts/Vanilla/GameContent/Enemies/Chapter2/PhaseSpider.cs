#nullable enable  
  
using MVZ2.GameContent.Effects;  
using MVZ2.Vanilla.Audios;  
using MVZ2.Vanilla.Entities;  
using MVZ2.Vanilla.Properties;  
using PVZEngine;  
using PVZEngine.Damages;  
using PVZEngine.Entities;  
using PVZEngine.Level;  
using Tools;  
using UnityEngine;  
  
namespace MVZ2.GameContent.Enemies  
{  
    [EntityBehaviourDefinition(VanillaEnemyNames.PhaseSpider)]  
    public class PhaseSpider : AIEntityBehaviour  
    {  
        public PhaseSpider(string nsp, string name) : base(nsp, name)  
        {  
        }  
  
        public override void Init(Entity entity)  
        {  
            base.Init(entity);  
            // 初始化瞬移冷却计时器，240帧（8秒@30fps）  
            SetTeleportTimer(entity, new FrameTimer(TELEPORT_INTERVAL));  
            // 初始化瞬移RNG  
            SetTeleportRNG(entity, new RandomGenerator(entity.RNG.Next()));  
            // 初始化受伤瞬移冷却计时器，初始为0帧（立即过期，允许第一次受伤触发）  
            SetDamageTeleportCooldown(entity, new FrameTimer(0));  
        }  
  
        protected override void UpdateAI(Entity entity)  
        {  
            base.UpdateAI(entity);  
            if (entity.IsDead)  
                return;  
  
            // 推进受伤瞬移冷却计时器（用固定值1，不受攻速影响）  
            var damageCooldown = GetDamageTeleportCooldown(entity);  
            damageCooldown?.Run(1);  
  
            // 周期性瞬移  
            var timer = GetTeleportTimer(entity);  
            if (timer != null)  
            {  
                timer.Run(entity.GetAttackSpeed());  
                if (timer.Expired)  
                {  
                    PerformTeleport(entity);  
                    timer.ResetTime(TELEPORT_INTERVAL);  
                }  
            }  
        }  
  
        public override void PostTakeDamage(DamageOutput result)  
        {  
            base.PostTakeDamage(result);  
            var bodyResult = result.BodyResult;  
            if (bodyResult == null || bodyResult.Amount <= 0)  
                return;  
  
            var entity = bodyResult.Entity;  
            if (entity == null || entity.IsDead)  
                return;  
  
            // 检查受伤瞬移冷却是否已过期  
            var damageCooldown = GetDamageTeleportCooldown(entity);  
            if (damageCooldown == null || !damageCooldown.Expired)  
                return;  
  
            // 受伤时75%概率瞬移  
            var rng = GetTeleportRNG(entity) ?? entity.RNG;  
            if (rng.Next(100) < DAMAGE_TELEPORT_CHANCE)  
            {  
                PerformTeleport(entity);  
                // 重置受伤瞬移冷却  
                damageCooldown.ResetTime(DAMAGE_TELEPORT_COOLDOWN);  
                // 受伤瞬移后重置周期计时器，避免连续瞬移  
                var timer = GetTeleportTimer(entity);  
                timer?.ResetTime(TELEPORT_INTERVAL);  
            }  
        }  
  
        /// <summary>  
        /// 执行瞬移：以自身为中心，随机方向+随机距离生成位移向量，直接传送过去  
        /// </summary>  
        private static void PerformTeleport(Entity entity)  
        {  
            var level = entity.Level;  
            var rng = GetTeleportRNG(entity) ?? entity.RNG;  
  
            // 随机角度 (0~360度) + 随机距离  
            float angle = rng.Next(0f, 360f);  
            float distance = rng.Next(MIN_TELEPORT_DIST, MAX_TELEPORT_DIST);  
  
            // 生成位移向量（XZ平面）  
            Vector3 displacement = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;  
  
            // 计算目标位置  
            Vector3 targetPos = entity.Position + displacement;  
  
            // 限制在关卡有效范围内  
            float minX = level.GetEntityColumnX(0);  
            float maxX = level.GetEntityColumnX(level.GetMaxColumnCount());  
            float minZ = level.GetGridBottomZ();  
            float maxZ = level.GetGridTopZ();  
  
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);  
            targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);  
            targetPos.y = level.GetGroundY(targetPos.x, targetPos.z);  
  
            // 原位置烟雾特效  
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter());  
  
            // 执行瞬移  
            entity.Position = targetPos;  
            entity.Velocity = Vector3.zero;  
  
            // 新位置烟雾特效  
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter());  
  
            // 播放传送音效  
            entity.PlaySound(VanillaSoundID.EndermanTeleport);  
        }  
  
        // 瞬移距离范围（世界坐标单位）  
        // 参考：一个格子宽度约 80 单位，一行高度约 100 单位  
        public const float MIN_TELEPORT_DIST = 20f;  
        public const float MAX_TELEPORT_DIST = 80f;  
  
        #region 属性存取  
        public static FrameTimer? GetTeleportTimer(Entity entity) =>  
            entity.GetBehaviourField<FrameTimer>(ID, PROP_TELEPORT_TIMER);  
        public static void SetTeleportTimer(Entity entity, FrameTimer value) =>  
            entity.SetBehaviourField(ID, PROP_TELEPORT_TIMER, value);  
  
        public static RandomGenerator? GetTeleportRNG(Entity entity) =>  
            entity.GetBehaviourField<RandomGenerator>(ID, PROP_TELEPORT_RNG);  
        public static void SetTeleportRNG(Entity entity, RandomGenerator value) =>  
            entity.SetBehaviourField(ID, PROP_TELEPORT_RNG, value);  
  
        public static FrameTimer? GetDamageTeleportCooldown(Entity entity) =>  
            entity.GetBehaviourField<FrameTimer>(ID, PROP_DAMAGE_TELEPORT_COOLDOWN);  
        public static void SetDamageTeleportCooldown(Entity entity, FrameTimer value) =>  
            entity.SetBehaviourField(ID, PROP_DAMAGE_TELEPORT_COOLDOWN, value);  
        #endregion  
  
        #region 常量  
        private static readonly NamespaceID ID = VanillaEnemyID.PhaseSpider;  
  
        // 周期瞬移间隔（帧数），约8秒  
        public const int TELEPORT_INTERVAL = 240;  
        // 受伤瞬移概率（百分比）  
        public const int DAMAGE_TELEPORT_CHANCE = 75;  
        // 受伤瞬移最小间隔（帧数）
        public const int DAMAGE_TELEPORT_COOLDOWN = 45;  
  
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_TELEPORT_TIMER =  
            new VanillaEntityPropertyMeta<FrameTimer>("TeleportTimer");  
        public static readonly VanillaEntityPropertyMeta<RandomGenerator> PROP_TELEPORT_RNG =  
            new VanillaEntityPropertyMeta<RandomGenerator>("TeleportRNG");  
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_DAMAGE_TELEPORT_COOLDOWN =  
            new VanillaEntityPropertyMeta<FrameTimer>("DamageTeleportCooldown");  
        #endregion  
    }  
}