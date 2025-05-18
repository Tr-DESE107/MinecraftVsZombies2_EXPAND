using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.tnt)]
    public class TNT : ContraptionBehaviour
    {
        public TNT(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback, filter: EntityTypes.PLANT);
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);

            SetExplosionTimer(entity, new FrameTimer(30));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (IsIgnited(entity))
            {
                IgnitedUpdate(entity);
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            if (result.BodyResult == null)
                return;
            if (result.BodyResult.Effects.HasEffect(VanillaDamageEffects.LIGHTNING))
            {
                Charge(result.Entity);
            }
        }
        public override bool CanTrigger(Entity entity)
        {
            return base.CanTrigger(entity) && !IsIgnited(entity);
        }
        protected override void OnTrigger(Entity entity)
        {
            base.OnTrigger(entity);
            Ignite(entity);
        }
        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);
            entity.SetEvoked(true);
            Ignite(entity);
        }
        private void PostEntityDeathCallback(LevelCallbacks.PostEntityDeathParams param, CallbackResult result)
        {
            var entity = param.entity;
            var info = param.deathInfo;
            if (!entity.IsEntityOf(VanillaContraptionID.tnt))
                return;
            if (!info.HasEffect(VanillaDamageEffects.SACRIFICE) &&
                !info.HasEffect(VanillaDamageEffects.FIRE) &&
                !info.HasEffect(VanillaDamageEffects.EXPLOSION))
                return;
            var range = entity.GetRange();
            var damage = entity.GetDamage();
            Explode(entity, range, damage);
            entity.Remove();
        }
        public static void Ignite(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.fuse);
            entity.SetBehaviourField(ID, PROP_IGNITED, true);
            entity.AddBuff<TNTIgnitedBuff>();
        }
        public static bool IsIgnited(Entity entity)
        {
            return entity.GetBehaviourField<bool>(ID, PROP_IGNITED);
        }
        public static void Charge(Entity tnt)
        {
            if (tnt.HasBuff<TNTChargedBuff>())
                return;
            tnt.AddBuff<TNTChargedBuff>();
        }
        public static bool IsCharged(Entity tnt)
        {
            return tnt.HasBuff<TNTChargedBuff>();
        }
        public static FrameTimer GetExplosionTimer(Entity entity)
        {
            return entity.GetBehaviourField<FrameTimer>(ID, PROP_EXPLOSION_TIMER);
        }
        public static void SetExplosionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetBehaviourField(ID, PROP_EXPLOSION_TIMER, timer);
        }
        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                var result = output?.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            var explosionParam = entity.GetSpawnParams();
            explosionParam.SetProperty(EngineEntityProps.SIZE, Vector3.one * (range * 2));
            var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), explosionParam);
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);

            if (IsCharged(entity))
            {
                ChargedExplode(entity);
            }
            entity.Level.Triggers.RunCallbackFiltered(VanillaLevelCallbacks.POST_CONTRAPTION_DETONATE, new EntityCallbackParams(entity), entity.GetDefinitionID());

            return damageOutputs;
        }
        private void IgnitedUpdate(Entity entity)
        {
            var timer = GetExplosionTimer(entity);
            timer.Run(entity.GetAttackSpeed());

            if (timer.Frame < 5)
            {
                entity.SetDisplayScale(Vector3.one * Mathf.Lerp(2, 1, timer.Frame / 5f));
            }
            if (timer.Expired)
            {
                var range = entity.GetRange();
                var damage = entity.GetDamage();
                Explode(entity, range, damage);
                if (entity.IsEvoked())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var direction = Quaternion.Euler(0, i * 90, 0) * Vector3.right * 10;
                        var velocity = direction;
                        velocity.y = 10;
                        var projectile = entity.ShootProjectile(VanillaProjectileID.flyingTNT, velocity);
                        projectile.SetDamage(damage);
                        projectile.SetRange(range);
                        if (IsCharged(entity))
                        {
                            Charge(projectile);
                        }
                    }
                }
                entity.Remove();
            }
        }
        public static void ExplodeArcs(Entity entity, Vector3 position, float arcLength = 1000)
        {
            var level = entity.Level;
            for (int i = 0; i < 18; i++)
            {
                var arc = level.Spawn(VanillaEffectID.electricArc, position, entity);

                float degree = i * 20;
                float rad = degree * Mathf.Deg2Rad;
                Vector3 pos = position + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * arcLength;
                ElectricArc.Connect(arc, pos);
                ElectricArc.UpdateArc(arc);
            }
        }
        private static void ChargedExplode(Entity entity)
        {
            ExplodeArcs(entity, entity.Position);
            var level = entity.Level;
            foreach (Entity unit in level.FindEntities(e => entity.IsHostile(e)))
            {
                unit.TakeDamage(entity.GetDamage(), new DamageEffectList(VanillaDamageEffects.LIGHTNING, VanillaDamageEffects.IGNORE_ARMOR), entity);
                if (unit.IsEntityOf(VanillaBossID.frankenstein))
                {
                    Frankenstein.Paralyze(unit, entity);
                }
            }
            entity.PlaySound(VanillaSoundID.thunder);
        }
        private static readonly NamespaceID ID = VanillaContraptionID.tnt;
        public static readonly VanillaEntityPropertyMeta PROP_IGNITED = new VanillaEntityPropertyMeta("Ignited");
        public static readonly VanillaEntityPropertyMeta PROP_EXPLOSION_TIMER = new VanillaEntityPropertyMeta("ExplosionTimer");
    }
}
