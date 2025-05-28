﻿using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.seijaMagicBomb)]
    public class SeijaMagicBomb : ProjectileBehaviour
    {
        public SeijaMagicBomb(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMaskHostile = 0;
            entity.CollisionMaskFriendly = 0;
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var range = entity.GetRange();
            var damage = entity.GetDamage();

            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Explode(entity.Position, range, entity.GetFaction(), entity.GetDamage(), damageEffects);
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
            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, Vector3.one * (range * 2 / 100f));
            var explosion = entity.Spawn(VanillaEffectID.magicBombExplosion, entity.GetCenter(), param);
            entity.PlaySound(VanillaSoundID.evocation);
        }
    }
}
