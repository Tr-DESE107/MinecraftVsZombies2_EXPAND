﻿using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.talismanZombie)]
    public class TalismanZombie : MeleeEnemy
    {
        public TalismanZombie(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEntityTakeDamageCallback);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetMoveTimer(entity, new FrameTimer(MOVE_INTERVAL));
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
        }
        protected override void WalkUpdate(Entity enemy)
        {
            var timer = GetMoveTimer(enemy);
            timer.Run();
            if (timer.Expired)
            {
                if (enemy.IsOnGround)
                {
                    timer.Reset();
                    var vel = enemy.GetFacingDirection() * enemy.GetSpeed() * 1;
                    vel.y = 5;
                    enemy.Velocity += vel;
                }
            }
        }
        private void PostEntityTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult callbackResult)
        {
            var output = param.output;
            var level = output.Entity.Level;
            foreach (var result in output.GetAllResults())
            {
                if (result == null)
                    continue;
                if (!result.HasEffect(VanillaDamageEffects.ENEMY_MELEE))
                    continue;
                var source = result.Source?.GetEntity(level);
                if (source == null)
                    continue;
                if (!source.Definition.HasBehaviour(this))
                    return;
                source.HealEffects(result.Amount, source);
            }
        }
        public static void SetMoveTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(PROP_MOVE_TIMER, timer);
        }
        public static FrameTimer GetMoveTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(PROP_MOVE_TIMER);
        }
        public const int MOVE_INTERVAL = 30;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_MOVE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("MoveTimer");
    }
}
