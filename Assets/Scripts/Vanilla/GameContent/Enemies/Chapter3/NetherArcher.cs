#nullable enable

using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.NetherArcher)]
    public class NetherArcher : AIEntityBehaviour
    {
        public NetherArcher(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true,
                ignoreLowEnemy = true
            };
        }

        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);

            // 寻找目标
            if (CanShoot(enemy))
            {
                if (enemy.Target != null && !ValidateTarget(enemy, enemy.Target))
                    enemy.Target = null;

                enemy.Target = FindTarget(enemy);
            }
            else
            {
                enemy.Target = null;
            }

            // 根据状态执行AI行为
            switch (enemy.State)
            {
                case STATE_RANGED_ATTACK:
                    PullBow(enemy);
                    break;
                default:
                    UnleaseBow(enemy);
                    break;
            }
        }

        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationFloat("BowBlend", 1 - Mathf.Pow(1 - GetBowPower(entity) / (float)BOW_POWER_MAX, 2));
            entity.SetAnimationBool("ArrowVisible", !GetBowFired(entity));
        }

        // -------------------------------------
        // 逻辑部分
        // -------------------------------------

        private void PullBow(Entity entity)
        {
            bool bowFired = GetBowFired(entity);
            int bowPower = GetBowPower(entity);

            if (!bowFired)
            {
                bowPower += (int)(entity.GetAttackSpeed() * BOW_POWER_PULL_SPEED);
                if (bowPower >= BOW_POWER_MAX)
                {
                    bowPower = BOW_POWER_MAX;
                    SetBowFired(entity, true);

                    ShootFireArrow(entity); // 发射火箭
                }
            }
            else
            {
                // 弓已发射，开始回位
                bowPower -= (int)(entity.GetAttackSpeed() * BOW_POWER_RESTORE_SPEED);
                if (bowPower <= 0)
                {
                    bowPower = 0;
                    SetBowFired(entity, false);
                }
            }

            SetBowPower(entity, bowPower);
        }

        private void UnleaseBow(Entity enemy)
        {
            SetBowFired(enemy, false);
            int bowPower = GetBowPower(enemy);

            if (bowPower > 0)
            {
                bowPower -= (int)(enemy.GetAttackSpeed() * BOW_POWER_RESTORE_SPEED);
                bowPower = Mathf.Max(bowPower, 0);
            }

            SetBowPower(enemy, bowPower);
        }

        // -------------------------------------
        // 射击逻辑
        // -------------------------------------
        private void ShootFireArrow(Entity entity)
        {
            var param = entity.GetShootParams();
            param.projectileID = VanillaProjectileID.arrow;

            var arrow = entity.ShootProjectile(param);

            // 无论血量如何，都添加基础火焰效果
            var fireBuff = arrow.AddBuff<HellfireIgnitedBuff>();

            if (entity.Health <= entity.GetMaxHealth() / 2)
            {
                // 半血时强化效果
                HellfireIgnitedBuff.Curse(fireBuff);

                // 轻微自伤，平衡机制
                entity.TakeDamage(
                    10,
                    new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.NO_DAMAGE_BLINK, VanillaDamageEffects.IGNORE_ARMOR),
                    entity
                );
            }
        }

        // -------------------------------------
        // 工具函数与属性
        // -------------------------------------
        private bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }

        private Entity? FindTarget(Entity entity)
        {
            var collider = detector.Detect(entity);
            return collider?.Entity;
        }

        private bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }

        // -------------------------------------
        // Bow状态字段
        // -------------------------------------
        public static int GetBowPower(Entity enemy) => enemy.GetBehaviourField<int>(ID, PROP_BOW_POWER);
        public static bool GetBowFired(Entity enemy) => enemy.GetBehaviourField<bool>(ID, PROP_BOW_FIRED);
        public static void SetBowPower(Entity enemy, int value) => enemy.SetBehaviourField(ID, PROP_BOW_POWER, value);
        public static void SetBowFired(Entity enemy, bool value) => enemy.SetBehaviourField(ID, PROP_BOW_FIRED, value);

        // -------------------------------------
        // 常量与字段
        // -------------------------------------
        private Detector detector;
        private static readonly NamespaceID ID = VanillaEnemyID.NetherArcher;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_BOW_FIRED = new VanillaEntityPropertyMeta<bool>("bowFired");
        public static readonly VanillaEntityPropertyMeta<int> PROP_BOW_POWER = new VanillaEntityPropertyMeta<int>("bowPower");
        public const int STATE_WALK = VanillaEnemyStates.WALK;
        public const int STATE_RANGED_ATTACK = VanillaEnemyStates.RANGED_ATTACK;
        public const int BOW_POWER_PULL_SPEED = 100;
        public const int BOW_POWER_RESTORE_SPEED = 1000;
        public const int BOW_POWER_MAX = 10000;
    }
}
