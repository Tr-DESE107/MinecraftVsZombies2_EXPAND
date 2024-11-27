using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Projectiles;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Grids;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [Definition(VanillaContraptionNames.tnt)]
    [EntitySeedDefinition(150, VanillaMod.spaceName, VanillaRechargeNames.veryLongTime)]
    public class TNT : VanillaContraption
    {
        public TNT(string nsp, string name) : base(nsp, name)
        {
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
        public static Entity[] Explode(Entity entity, float range, float damage)
        {
            var entities = entity.Level.Explode(entity.Position, range, entity.GetFaction(), damage, new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.IGNORE_ARMOR), new EntityReferenceChain(entity));
            foreach (var e in entities)
            {
                if (e.IsDead)
                {
                    var distance = (e.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    e.Velocity = e.Velocity + Vector3.up * speed;
                }
            }
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetBoundsCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);
            return entities;
        }
        private void IgnitedUpdate(Entity entity)
        {
            var timer = GetExplosionTimer(entity);
            timer.Run(entity.GetAttackSpeed());

            if (timer.Frame < 5)
            {
                entity.RenderScale = Vector3.one * Mathf.Lerp(2, 1, timer.Frame / 5f);
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
                        entity.ShootProjectile(VanillaProjectileID.flyingTNT, velocity);
                    }
                }
                entity.Remove();
            }
        }
    }
}
