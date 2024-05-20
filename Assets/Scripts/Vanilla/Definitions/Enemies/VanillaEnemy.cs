using MVZ2.GameContent;
using MVZ2.Vanilla.Buffs;
using PVZEngine;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.Vanilla
{
    public abstract class VanillaEnemy : EntityDefinition
    {
        public VanillaEnemy()
        {
            SetProperty(EnemyProps.SPEED, 0.5f);
            SetProperty(EntityProperties.SHELL, ShellID.flesh);
            SetProperty(EntityProperties.ATTACK_SPEED, 1f);
            SetProperty(EntityProperties.DAMAGE, 100f);
            SetProperty(EntityProperties.MAX_HEALTH, 200f);
            SetProperty(EntityProperties.FALL_DAMAGE, 22.5f);
            SetProperty(EntityProperties.FRICTION, 0.15f);
            SetProperty(EntityProps.DEATH_SOUND, SoundID.zombieDeath);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.Game.CreateBuff<RandomEnemySpeedBuff>();
            buff.SetProperty(RandomEnemySpeedBuff.PROP_SPEED, entity.RNG.Next(1, 1.33333f));
            entity.AddBuff(buff);

            entity.SetFaction(entity.Game.Option.RightFaction);

            entity.CollisionMask = EntityCollision.MASK_CONTRAPTION
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS
                | EntityCollision.MASK_HOSTILE;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            Vector3 pos = entity.Pos;
            pos.x = Mathf.Min(pos.x, MVZ2Game.GetEnemyRightBorderX());
            entity.Pos = pos;

            if (!entity.IsDead)
            {
                foreach (var buff in entity.GetBuffs<DamageColorBuff>())
                {
                    entity.RemoveBuff(buff);
                }
            }
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff(entity.Game.CreateBuff<DamageColorBuff>());
            }
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            entity.Game.PlaySound(entity.GetDeathSound(), entity.Pos);
        }
        protected void MeleeCollision(Enemy enemy, Entity other)
        {
            if (ValidateMeleeTarget(enemy, enemy.AttackTarget))
                return;
            if (ValidateMeleeTarget(enemy, other))
            {
                enemy.AttackTarget = other;
            }
        }
        protected void CancelMeleeAttack(Enemy enemy, Entity other)
        {
            if (enemy.AttackTarget == other)
            {
                enemy.AttackTarget = null;
            }
        }
        protected virtual bool ValidateMeleeTarget(Enemy enemy, Entity target)
        {
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            if (!enemy.IsEnemy(target))
                return false;
            if (!Detection.IsInSameRow(enemy, target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (target is Contraption contrap && contrap.IsFloor())
                return false;
            if (target.Pos.y > enemy.Pos.y + enemy.GetMaxAttackHeight())
                return false;
            return true;
        }
        public override int Type => EntityTypes.ENEMY;
    }
    public abstract class MeleeEnemy : VanillaEnemy
    {
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var enemy = entity.ToEnemy();
            if (!ValidateMeleeTarget(enemy, enemy.AttackTarget))
                enemy.AttackTarget = null;
            enemy.ActionState = GetActionState(enemy);
            UpdateActionState(enemy, enemy.ActionState);

        }
        public override void PostCollision(Entity entity, Entity other, int state)
        {
            var enemy = entity.ToEnemy();
            if (state != EntityCollision.STATE_EXIT)
            {
                MeleeCollision(enemy, other);
            }
            else
            {
                CancelMeleeAttack(enemy, other);
            }
        }
        public override void PostDeath(Entity entity, DamageInfo info)
        {
            base.PostDeath(entity, info);
            SetDeathTimer(entity, new FrameTimer(30));
        }
        public static FrameTimer GetDeathTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("DeathTimer");
        }
        public static void SetDeathTimer(Entity entity, FrameTimer frameTimer)
        {
            entity.SetProperty("DeathTimer", frameTimer);
        }
        protected virtual int GetActionState(Enemy enemy)
        {
            if (enemy.IsDead)
            {
                return EnemyStates.DEAD;
            }
            else if (enemy.IsPreview)
            {
                return EnemyStates.IDLE;
            }
            else if (enemy.AttackTarget != null)
            {
                return EnemyStates.ATTACK;
            }
            else
            {
                return EnemyStates.WALK;
            }
        }
        protected virtual void UpdateActionState(Enemy enemy, int state)
        {
            enemy.SetAnimationInt("State", state);
            switch (state)
            {
                case EnemyStates.WALK:
                    UpdateStateWalk(enemy);
                    break;
                case EnemyStates.ATTACK:
                    UpdateStateAttack(enemy);
                    break;
                case EnemyStates.DEAD:
                    UpdateStateDead(enemy);
                    break;
                case EnemyStates.CAST:
                    UpdateStateCast(enemy);
                    break;
                case EnemyStates.IDLE:
                    UpdateStateIdle(enemy);
                    break;
            }
        }
        protected virtual void UpdateStateWalk(Enemy enemy)
        {
            var velocity = enemy.Velocity;
            var speed = enemy.GetSpeed();
            if (Mathf.Abs(velocity.x) < speed)
            {
                float min = Mathf.Min(speed, -speed);
                float max = Mathf.Max(speed, -speed);
                float direciton = enemy.IsFacingLeft() ? -1 : 1;
                velocity.x += speed * direciton;
                velocity.x = Mathf.Clamp(velocity.x, min, max);
            }
            enemy.Velocity = velocity;
        }
        protected virtual void UpdateStateAttack(Enemy enemy)
        {
            MeleeAttack(enemy, enemy.AttackTarget);
        }
        protected virtual void UpdateStateDead(Enemy enemy)
        {
            var deathTimer = GetDeathTimer(enemy);
            if (deathTimer == null)
            {
                deathTimer = new FrameTimer(30);
                SetDeathTimer(enemy, deathTimer);
            }
            deathTimer.Run();
            if (deathTimer.Expired)
            {
                enemy.Remove();
            }
        }
        protected virtual void UpdateStateCast(Enemy enemy)
        {
        }
        protected virtual void UpdateStateIdle(Enemy enemy)
        {
        }
        protected void MeleeAttack(Enemy enemy, Entity target)
        {
            if (target == null)
                return;
            target.TakeDamage(enemy.GetDamage() * enemy.GetAttackSpeed() / 30f, new DamageEffectList(DamageEffects.MUTE), new EntityReference(enemy));
        }
    }

}