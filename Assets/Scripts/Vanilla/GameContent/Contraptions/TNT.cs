using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.tnt)]
    public class TNT : ContraptionBehaviour
    {
        public TNT(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_SACRIFICE, PostSacrificeCallback, filter: VanillaContraptionID.tnt);
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
        public static void Ignite(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.fuse);
            entity.SetProperty("Ignited", true);
            entity.AddBuff<TNTIgnitedBuff>();
        }
        public static bool IsIgnited(Entity entity)
        {
            return entity.GetProperty<bool>("Ignited");
        }
        public static FrameTimer GetExplosionTimer(Entity entity)
        {
            return entity.GetProperty<FrameTimer>("ExplosionTimer");
        }
        public static void SetExplosionTimer(Entity entity, FrameTimer timer)
        {
            entity.SetProperty("ExplosionTimer", timer);
        }
        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                if (output.BodyResult.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);

            if (entity.HasBuff<TNTChargedBuff>())
            {
                ChargedExplode(entity);
            }
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
                        if (entity.HasBuff<TNTChargedBuff>())
                        {
                            projectile.AddBuff<TNTChargedBuff>();
                        }
                    }
                }
                entity.Remove();
            }
        }
        private void PostSacrificeCallback(Entity entity, Entity soulFurnace, int fuel)
        {
            var range = entity.GetRange();
            var damage = entity.GetDamage();
            Explode(entity, range, damage);
            entity.Remove();
        }
        private static void ChargedExplode(Entity entity)
        {
            const float arcLength = 1000;
            var level = entity.Level;
            for (int i = 0; i < 18; i++)
            {
                var arc = level.Spawn(VanillaEffectID.electricArc, entity.Position, entity);

                float degree = i * 20;
                float rad = degree * Mathf.Deg2Rad;
                Vector3 pos = entity.Position + new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * arcLength;
                ElectricArc.Connect(arc, pos);
            }
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
    }
}
