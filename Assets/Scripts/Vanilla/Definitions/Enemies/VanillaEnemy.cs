using MVZ2.Extensions;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    public abstract class VanillaEnemy : VanillaEntity
    {
        public VanillaEnemy(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinEnemyProps.SPEED, 0.5f);
            SetProperty(BuiltinEnemyProps.CRY_SOUND, SoundID.zombieCry);
            SetProperty(EntityProperties.SHELL, ShellID.flesh);
            SetProperty(EntityProperties.ATTACK_SPEED, 1f);
            SetProperty(EntityProperties.DAMAGE, 100f);
            SetProperty(EntityProperties.MAX_HEALTH, 200f);
            SetProperty(EntityProperties.FALL_DAMAGE, 22.5f);
            SetProperty(EntityProperties.FRICTION, 0.15f);
            SetProperty(EntityProperties.FACE_LEFT_AT_DEFAULT, true);
            SetProperty(BuiltinEntityProps.DEATH_SOUND, SoundID.zombieDeath);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            var buff = entity.Level.CreateBuff<RandomEnemySpeedBuff>();
            buff.SetProperty(RandomEnemySpeedBuff.PROP_SPEED, GetRandomSpeedMultiplier(entity));
            entity.AddBuff(buff);

            entity.SetFaction(entity.Level.Option.RightFaction);

            entity.CollisionMask = EntityCollision.MASK_PLANT
                | EntityCollision.MASK_ENEMY
                | EntityCollision.MASK_OBSTACLE
                | EntityCollision.MASK_BOSS;
        }
        public override void Update(Entity enemy)
        {
            base.Update(enemy);
            Vector3 pos = enemy.Pos;
            pos.x = Mathf.Min(pos.x, BuiltinLevel.GetEnemyRightBorderX());
            enemy.Pos = pos;

            enemy.SetAnimationFloat("AttackSpeed", enemy.GetAttackSpeed());
            enemy.SetAnimationFloat("MoveSpeed", enemy.GetSpeed() * 2);
        }
        public override void PostTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostTakeDamage(bodyResult, armorResult);
            if (bodyResult != null)
            {
                var entity = bodyResult.Entity;
                if (!entity.HasBuff<DamageColorBuff>())
                    entity.AddBuff<DamageColorBuff>();
            }
        }
        public override void PostDeath(Entity entity, DamageInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            if (damageInfo.Effects.HasEffect(DamageEffects.REMOVE_ON_DEATH))
            {
                entity.Remove();
                return;
            }
            entity.PlaySound(entity.GetDeathSound());
        }
        protected void MeleeCollision(Entity enemy, Entity other)
        {
            if (ValidateMeleeTarget(enemy, enemy.Target))
                return;
            if (ValidateMeleeTarget(enemy, other))
            {
                enemy.Target = other;
            }
        }
        protected void CancelMeleeAttack(Entity enemy, Entity other)
        {
            if (enemy.Target == other)
            {
                enemy.Target = null;
            }
        }
        protected virtual bool ValidateMeleeTarget(Entity enemy, Entity target)
        {
            if (target == null || !target.Exists() || target.IsDead)
                return false;
            if (!enemy.IsEnemy(target))
                return false;
            if (!Detection.IsInSameRow(enemy, target))
                return false;
            if (!Detection.CanDetect(target))
                return false;
            if (target.Type == EntityTypes.PLANT && target.IsFloor())
                return false;
            if (target.Pos.y > enemy.Pos.y + enemy.GetMaxAttackHeight())
                return false;
            return true;
        }
        protected virtual float GetRandomSpeedMultiplier(Entity entity)
        {
            return entity.RNG.Next(1, 1.33333f);
        }
        public override int Type => EntityTypes.ENEMY;
    }
    public abstract class MeleeEnemy : VanillaEnemy
    {
        protected MeleeEnemy(string nsp, string name) : base(nsp, name)
        {
        }

        public override void Update(Entity enemy)
        {
            base.Update(enemy);
            if (!ValidateMeleeTarget(enemy, enemy.Target))
                enemy.Target = null;
            enemy.State = GetActionState(enemy);
            UpdateActionState(enemy, enemy.State);

        }
        public override void PostCollision(Entity enemy, Entity other, int state)
        {
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
        protected virtual int GetActionState(Entity enemy)
        {
            if (enemy.IsDead)
            {
                return EntityStates.DEAD;
            }
            else if (enemy.IsPreviewEnemy())
            {
                return EntityStates.IDLE;
            }
            else if (enemy.Target != null)
            {
                return EntityStates.ATTACK;
            }
            else
            {
                return EntityStates.WALK;
            }
        }
        protected virtual void UpdateActionState(Entity enemy, int state)
        {
            enemy.SetAnimationInt("State", state);
            switch (state)
            {
                case EntityStates.WALK:
                    UpdateStateWalk(enemy);
                    break;
                case EntityStates.ATTACK:
                    UpdateStateAttack(enemy);
                    break;
                case EntityStates.DEAD:
                    UpdateStateDead(enemy);
                    break;
                case EntityStates.CAST:
                    UpdateStateCast(enemy);
                    break;
                case EntityStates.IDLE:
                    UpdateStateIdle(enemy);
                    break;
            }
        }
        protected virtual void UpdateStateWalk(Entity enemy)
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
        protected virtual void UpdateStateAttack(Entity enemy)
        {
            MeleeAttack(enemy, enemy.Target);
        }
        protected virtual void UpdateStateDead(Entity enemy)
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
        protected virtual void UpdateStateCast(Entity enemy)
        {
        }
        protected virtual void UpdateStateIdle(Entity enemy)
        {
        }
        protected void MeleeAttack(Entity enemy, Entity target)
        {
            if (target == null)
                return;
            target.TakeDamage(enemy.GetDamage() * enemy.GetAttackSpeed() / 30f, new DamageEffectList(DamageEffects.MUTE), new EntityReferenceChain(enemy));
        }
    }

}