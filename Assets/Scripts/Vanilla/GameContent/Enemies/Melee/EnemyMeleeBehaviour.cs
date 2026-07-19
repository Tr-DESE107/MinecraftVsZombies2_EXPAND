#nullable enable // 自动生成

using System.Diagnostics.CodeAnalysis;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Entities;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEntityBehaviourNames.enemyMelee)]
    public class EnemyMeleeBehaviour : AIEntityBehaviour
    {
        public EnemyMeleeBehaviour(string nsp, string name) : base(nsp, name)
        {
        }

        #region 更新
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            // 检查近战是否启用
            if (!MeleeEnabled(entity))
                return;
            // 获取近战目标
            var targetID = GetMeleeTarget(entity);
            // 如果目标无效则清除
            if (targetID != null && !ValidateMeleeTarget(entity, targetID.GetEntity(entity.Level)))
            {
                SetMeleeTarget(entity, null);
            }

            if (entity.State == STATE_MELEE_ATTACK)
            {
                MeleeAttack(entity);
            }
        }
        #endregion

        #region 碰撞
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var entity = collision.Entity;
            if (!MeleeEnabled(entity))
                return;
            // 如果不是主碰撞体则忽略
            if (!collision.Collider.IsForMain() || !collision.OtherCollider.IsForMain())
                return;
            if (state != EntityCollisionHelper.STATE_EXIT)
            {
                CollisionStay(entity, collision.Other);
            }
            else
            {
                CollisionExit(entity, collision.Other);
            }
        }
        // 碰撞持续时
        private void CollisionStay(Entity enemy, Entity other)
        {
            // 获取当前近战目标
            var targetID = GetMeleeTarget(enemy);
            var currentTarget = targetID?.GetEntity(enemy.Level);
            // 如果当前目标仍然有效则返回
            if (ValidateMeleeTarget(enemy, currentTarget))
                return;
            // 如果新目标有效则设置为近战目标
            if (ValidateMeleeTarget(enemy, other))
            {
                SetMeleeTarget(enemy, new EntityID(other));
            }
        }
        // 碰撞退出时
        private void CollisionExit(Entity enemy, Entity other)
        {
            // 如果退出的是当前近战目标则清除
            var meleeTarget = GetMeleeTarget(enemy);
            if (meleeTarget != null && meleeTarget.ID == other.ID)
            {
                SetMeleeTarget(enemy, null);
            }
        }
        #endregion

        #region 验证目标
        protected virtual bool ValidateMeleeTarget(Entity enemy, [NotNullWhen(true)] Entity? target)
        {
            // 如果目标无效则返回false
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            // 检查是否为敌对目标
            if (!enemy.IsHostile(target))
                return false;
            // 检查是否可被检测
            if (!Detection.CanDetect(target))
                return false;
            // 检查目标高度是否在攻击范围内
            if (target.Position.y > enemy.Position.y + enemy.GetMaxAttackHeight())
                return false;
            // 近战方天然是怪物：对"仅对怪物隐身"的目标不可攻击  
            if (target.IsInvisibleToEnemy())
                return false;
            // 如果是植物或障碍物
            if (target.Type == EntityTypes.PLANT || target.Type == EntityTypes.OBSTACLE)
            {
                // 排除地板类实体
                if (target.IsFloor())
                    return false;
                // 检查是否有保护者
                var protector = target.GetProtector();
                if (protector != null && protector.Exists() && !protector.IsFriendly(enemy))
                    return false;
            }
            return true;
        }
        #endregion

        #region 近战攻击
        public virtual bool MeleeEnabled(Entity entity)
        {
            return true;
        }
        public static bool HasMeleeTarget(Entity enemy)
        {
            var meleeTarget = GetMeleeTarget(enemy);
            return meleeTarget != null && meleeTarget.Exists(enemy.Level);
        }
        public static bool IsMeleeAttacking(Entity entity)
        {
            return entity.State == STATE_MELEE_ATTACK;
        }
        public static void MeleeAttack(Entity enemy)
        {
            var meleeTargetID = GetMeleeTarget(enemy);
            var meleeTarget = meleeTargetID?.GetEntity(enemy.Level);
            if (meleeTarget != null)
            {
                MeleeAttack(enemy, meleeTarget);
            }
        }
        public static void MeleeAttack(Entity enemy, Entity target)
        {
            // 根据朝向减速
            var vel = enemy.Velocity;
            if (enemy.IsFacingLeft() == vel.x < 0)
            {
                vel.x *= 0.8f;
            }
            enemy.Velocity = vel;
            // 计算伤害（基础伤害 × 攻速 / 30帧）
            var damage = enemy.GetDamage() * enemy.GetAttackSpeed() / 30f;
            // 对目标造成伤害
            target.TakeDamage(damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.ENEMY_MELEE), enemy);
            // 触发近战攻击后回调
            enemy.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_ENEMY_MELEE_ATTACK, new VanillaLevelCallbacks.EnemyMeleeAttackParams(enemy, target, damage));
        }
        #endregion

        #region 属性
        public static EntityID? GetMeleeTarget(Entity entity) => entity.GetProperty<EntityID>(PROP_MELEE_TARGET);
        public static void SetMeleeTarget(Entity entity, EntityID? value) => entity.SetProperty(PROP_MELEE_TARGET, value);
        public static readonly VanillaEntityPropertyMeta<EntityID> PROP_MELEE_TARGET = new VanillaEntityPropertyMeta<EntityID>("melee_target");
        #endregion

        public const int STATE_MELEE_ATTACK = LogicEnemyStates.MELEE_ATTACK;
    }
}
