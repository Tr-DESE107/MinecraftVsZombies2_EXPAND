using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Callbacks;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.hoe)]
    public class Hoe : EffectBehaviour
    {

        #region 公有方法
        public Hoe(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LogicLevelCallbacks.POST_LEVEL_STOP, PostLevelStopCallback);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile = EntityCollisionHelper.MASK_ENEMY;
            SetStateTimer(entity, new FrameTimer(5));
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            if (!collision.Collider.IsMain() || !collision.OtherCollider.IsMain())
                return;
            if (state == EntityCollisionHelper.STATE_EXIT)
                return;
            var other = collision.Other;
            if (other.Type != EntityTypes.ENEMY)
                return;
            var hoe = collision.Entity;
            if (hoe.State != VanillaEntityStates.HOE_IDLE)
                return;
            if (!hoe.IsHostile(other))
                return;
            hoe.State = VanillaEntityStates.HOE_TRIGGERED;
            hoe.SetAnimationBool("Triggered", true);
            hoe.PlaySound(VanillaSoundID.swing);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            if (entity.State == VanillaEntityStates.HOE_TRIGGERED)
            {
                var timer = GetStateTimer(entity);
                timer.Run();
                if (timer.Expired)
                {
                    var target = entity.Level.FindFirstEntity(e => CanDamage(entity, e));
                    if (target != null)
                    {
                        target.Die(entity);
                        entity.PlaySound(VanillaSoundID.bonk);
                    }
                    entity.State = VanillaEntityStates.HOE_DAMAGED;
                    timer.ResetTime(30);
                }
            }
            else if (entity.State == VanillaEntityStates.HOE_DAMAGED)
            {
                var timer = GetStateTimer(entity);
                timer.Run();
                if (timer.Expired)
                {
                    var smoke = entity.Level.Spawn(VanillaEffectID.smoke, entity.Position, null);
                    smoke.SetSize(smoke.GetSize());
                    entity.Remove();
                }
            }
        }
        #endregion
        private void PostLevelStopCallback(LevelEngine level)
        {
            foreach (var hoe in level.FindEntities(e => e.IsEntityOf(VanillaEffectID.hoe)))
            {
                hoe.Remove();
            }
        }
        public static bool CanDamage(Entity hoe, Entity target)
        {
            return hoe.IsHostile(target) && target.Type == EntityTypes.ENEMY && hoe.MainHitbox.Intersects(target.MainHitbox);
        }
        public static void SetStateTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourProperty(ID, PROP_STATE_TIMER, timer);
        public static FrameTimer GetStateTimer(Entity entity) => entity.GetBehaviourProperty<FrameTimer>(ID, PROP_STATE_TIMER);

        public static readonly NamespaceID ID = VanillaEffectID.hoe;
        public const string PROP_STATE_TIMER = "StateTimer";
    }
}