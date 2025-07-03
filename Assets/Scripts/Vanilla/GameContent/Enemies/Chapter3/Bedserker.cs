using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.bedserker)]
    public class Bedserker : StateEnemy
    {
        public Bedserker(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetExplosionTimer(entity, new FrameTimer(EXPLOSION_TIMEOUT));
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            if (!entity.IsDead)
            {
                var explosionTimer = GetExplosionTimer(entity);
                explosionTimer.Run();
                if (explosionTimer.PassedFrame(30))
                {
                    entity.PlaySound(VanillaSoundID.fuse);
                }
                if (explosionTimer.PassedFrame(20))
                {
                    entity.PlaySound(VanillaSoundID.parabotTick);
                }
                if (explosionTimer.Expired)
                {
                    entity.Die(new DamageEffectList(VanillaDamageEffects.NO_NEUTRALIZE), entity);
                }
                Color color;
                if (explosionTimer.Frame <= 30 && explosionTimer.Frame % 4 < 2)
                {
                    color = Color.white;
                }
                else
                {
                    var x = Mathf.Pow((explosionTimer.MaxFrame - explosionTimer.Frame) / (explosionTimer.MaxFrame / 5f), 3);
                    var alpha = (-Mathf.Cos(x) + 1) * 0.25f;
                    color = new Color(1, 0, 0, alpha);
                }
                entity.SetProperty(PROP_COLOR_OFFSET, color);
            }
            entity.SetModelDamagePercent();
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            Berserker.Explode(entity, entity.GetDamage() * 18, VanillaFactions.NEUTRAL);
            entity.Level.ShakeScreen(20, 0, 30);
            entity.Remove();
        }
        public static void SetExplosionTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EXPLOSION_TIMER, timer);
        public static FrameTimer GetExplosionTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EXPLOSION_TIMER);
        public const int EXPLOSION_TIMEOUT = 300;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EXPLOSION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("ExplosionTimer");
        public static readonly VanillaEntityPropertyMeta<Color> PROP_COLOR_OFFSET = new VanillaEntityPropertyMeta<Color>("ColorOffset");
    }
}
