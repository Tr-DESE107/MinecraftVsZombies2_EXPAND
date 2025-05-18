using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.Vanilla.Enemies
{
    public abstract class StateEnemy : EnemyBehaviour
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
            if (entity.State == VanillaEntityStates.DEAD)
            {
                UpdateStateDead(entity);
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            SetDeathTimer(entity, new FrameTimer(30));
        }
        public static FrameTimer GetDeathTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(PROP_DEATH_TIMER);
        }
        public static void SetDeathTimer(Entity entity, FrameTimer frameTimer)
        {
            entity.SetBehaviourField(PROP_DEATH_TIMER, frameTimer);
        }
        protected virtual int GetActionState(Entity enemy)
        {
            if (enemy.IsDead)
            {
                return VanillaEntityStates.DEAD;
            }
            else if (enemy.IsPreviewEnemy())
            {
                return VanillaEntityStates.IDLE;
            }
            else if (enemy.Target != null)
            {
                return VanillaEntityStates.ATTACK;
            }
            else
            {
                return VanillaEntityStates.WALK;
            }
        }
        protected virtual void UpdateActionState(Entity enemy, int state)
        {
            enemy.SetAnimationInt("State", state);
            switch (state)
            {
                case VanillaEntityStates.WALK:
                    UpdateStateWalk(enemy);
                    break;
                case VanillaEntityStates.ATTACK:
                    UpdateStateAttack(enemy);
                    break;
                case VanillaEntityStates.ENEMY_CAST:
                    UpdateStateCast(enemy);
                    break;
                case VanillaEntityStates.IDLE:
                    UpdateStateIdle(enemy);
                    break;
            }
        }
        protected virtual void WalkUpdate(Entity enemy)
        {
            enemy.UpdateWalkVelocity();
        }
        protected virtual void UpdateStateWalk(Entity enemy)
        {
            WalkUpdate(enemy);
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
                enemy.FaintRemove();
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
        private const string PROP_REGION = "state_enemy";
        [EntityPropertyRegistry(PROP_REGION)]
        public static readonly VanillaEntityPropertyMeta PROP_DEATH_TIMER = new VanillaEntityPropertyMeta("DeathTimer");
    }

}