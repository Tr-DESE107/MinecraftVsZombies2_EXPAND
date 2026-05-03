#nullable enable

using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Definitions;
using Tools;
using UnityEngine;
using MVZ2Logic.Entities;
using MVZ2.GameContent.Entities;

namespace MVZ2.GameContent.Enemies  
{  
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.PhaseSpider)]  
    public class PhaseSpider : AIEntityBehaviour  
    {  
        public PhaseSpider(string nsp, string name) : base(nsp, name)  
        {  
        }  
  
        public override void Init(Entity entity)  
        {  
            base.Init(entity);  
            // ��ʼ��˲����ȴ��ʱ����240֡��8��@30fps��  
            SetTeleportTimer(entity, new FrameTimer(TELEPORT_INTERVAL));  
            // ��ʼ��˲��RNG  
            SetTeleportRNG(entity, new RandomGenerator(entity.RNG.Next()));  
            // ��ʼ������˲����ȴ��ʱ������ʼΪ0֡���������ڣ�������һ�����˴�����  
            SetDamageTeleportCooldown(entity, new FrameTimer(0));  
        }  
  
        protected override void UpdateAI(Entity entity)  
        {  
            base.UpdateAI(entity);  
            if (entity.IsDead)  
                return;  
  
            // �ƽ�����˲����ȴ��ʱ�����ù̶�ֵ1�����ܹ���Ӱ�죩  
            var damageCooldown = GetDamageTeleportCooldown(entity);  
            damageCooldown?.Run(1);  
  
            // ������˲��  
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
  
            // �������˲����ȴ�Ƿ��ѹ���  
            var damageCooldown = GetDamageTeleportCooldown(entity);  
            if (damageCooldown == null || !damageCooldown.Expired)  
                return;  
  
            // ����ʱ75%����˲��  
            var rng = GetTeleportRNG(entity) ?? entity.RNG;  
            if (rng.Next(100) < DAMAGE_TELEPORT_CHANCE)  
            {  
                PerformTeleport(entity);  
                // ��������˲����ȴ  
                damageCooldown.ResetTime(DAMAGE_TELEPORT_COOLDOWN);  
                // ����˲�ƺ��������ڼ�ʱ������������˲��  
                var timer = GetTeleportTimer(entity);  
                timer?.ResetTime(TELEPORT_INTERVAL);  
            }  
        }  
  
        /// <summary>  
        /// ִ��˲�ƣ�������Ϊ���ģ��������+�����������λ��������ֱ�Ӵ��͹�ȥ  
        /// </summary>  
        private static void PerformTeleport(Entity entity)  
        {  
            var level = entity.Level;  
            var rng = GetTeleportRNG(entity) ?? entity.RNG;  
  
            // ����Ƕ� (0~360��) + �������  
            float angle = rng.Next(0f, 360f);  
            float distance = rng.Next(MIN_TELEPORT_DIST, MAX_TELEPORT_DIST);  
  
            // ����λ��������XZƽ�棩  
            Vector3 displacement = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;  
  
            // ����Ŀ��λ��  
            Vector3 targetPos = entity.Position + displacement;  
  
            // �����ڹؿ���Ч��Χ��  
            float minX = level.GetEntityColumnX(0);  
            float maxX = level.GetEntityColumnX(level.GetMaxColumnCount());  
            float minZ = level.GetGridBottomZ();  
            float maxZ = level.GetGridTopZ();  
  
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);  
            targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);  
            targetPos.y = level.GetGroundY(targetPos.x, targetPos.z);  
  
            // ԭλ��������Ч  
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter());  
  
            // ִ��˲��  
            entity.Position = targetPos;  
            entity.Velocity = Vector3.zero;  
  
            // ��λ��������Ч  
            entity.Spawn(VanillaEffectID.smokeCluster, entity.GetCenter());  
  
            // ���Ŵ�����Ч  
            entity.PlaySound(VanillaSoundID.EndermanTeleport);  
        }  
  
        // ˲�ƾ��뷶Χ���������굥λ��  
        // �ο���һ�����ӿ���Լ 80 ��λ��һ�и߶�Լ 100 ��λ  
        public const float MIN_TELEPORT_DIST = 20f;  
        public const float MAX_TELEPORT_DIST = 80f;  
  
        #region ���Դ�ȡ  
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
  
        #region ����  
        private static readonly NamespaceID ID = VanillaEnemyID.PhaseSpider;  
  
        // ����˲�Ƽ����֡������Լ8��  
        public const int TELEPORT_INTERVAL = 240;  
        // ����˲�Ƹ��ʣ��ٷֱȣ�  
        public const int DAMAGE_TELEPORT_CHANCE = 75;  
        // ����˲����С�����֡����
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