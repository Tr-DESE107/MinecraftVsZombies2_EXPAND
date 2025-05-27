using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.compellingOrb)]
    public class CompellingOrb : ProjectileBehaviour
    {
        public CompellingOrb(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(30));
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            if (projectile.Target == null || !projectile.Target.Exists() || projectile.Target.IsDead ||
                projectile.Parent == null || !projectile.Parent.Exists() || projectile.Parent.IsDead)
            {
                projectile.Die();
                return;
            }

            if (projectile.State == STATE_IDLE)
            {
                var timer = GetStateTimer(projectile);
                timer.Run();
                if (timer.Expired)
                {
                    projectile.State = STATE_FLY;
                    projectile.Velocity = (projectile.Target.GetCenter() - projectile.GetCenter()).normalized * 20;
                }
            }
            projectile.RenderRotation += Vector3.forward * 10;
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var projectile = hitResult.Projectile;
            if (hitResult.Shield != null)
                return;
            var target = hitResult.Other;
            if (!CanControl(target))
            {
                target.PlaySound(VanillaSoundID.mindClear);
                return;
            }
            target.CharmWithSource(projectile.Parent);
            target.PlaySound(VanillaSoundID.mindControl);
        }
        public override void PostDeath(Entity entity, DeathInfo damageInfo)
        {
            base.PostDeath(entity, damageInfo);
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.TINT, Color.magenta);
            param.SetProperty(EngineEntityProps.SIZE, entity.GetScaledSize());
            entity.Spawn(VanillaEffectID.smoke, entity.Position, param);
        }
        public static bool CanControl(Entity target)
        {
            return !target.IsLoyal() && !target.IsCharmed() && (target.Type == EntityTypes.PLANT || target.Type == EntityTypes.ENEMY || target.Type == EntityTypes.OBSTACLE);
        }
        public static void SetStateTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_STATE_TIMER, timer);
        public static FrameTimer GetStateTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_STATE_TIMER);
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        public const int STATE_FLY = VanillaEntityStates.WALK;

        private static readonly NamespaceID ID = VanillaProjectileID.compellingOrb;
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateTimer");
    }
}
