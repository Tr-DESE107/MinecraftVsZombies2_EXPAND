using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla.Enemies
{
    public abstract class StateEnemy : VanillaEnemy
    {
        protected StateEnemy(string nsp, string name) : base(nsp, name)
        {
        }

        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            entity.State = GetActionState(entity);
            UpdateActionState(entity, entity.State);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (entity.State == EntityStates.DEAD)
            {
                UpdateStateDead(entity);
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
            var speed = enemy.GetSpeed() * 0.4f;
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
                var smoke = enemy.Level.Spawn(VanillaEffectID.smoke, enemy.Position, enemy);
                smoke.SetSize(enemy.GetSize());
                enemy.Remove();
            }
        }
        protected virtual void UpdateStateAttack(Entity enemy)
        {
        }
        protected virtual void UpdateStateCast(Entity enemy)
        {
        }
        protected virtual void UpdateStateIdle(Entity enemy)
        {
        }
    }

}