using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;


namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.RedStoneOre)]
    public class RedStoneOre : ContraptionBehaviour
    {
        public RedStoneOre(string nsp, string name) : base(nsp, name)
        {

        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            var Orecount = 12;
            if (entity.Level.Difficulty == VanillaDifficulties.normal)
            {
                Orecount = 10;
            }
            else if (entity.Level.Difficulty == VanillaDifficulties.hard)
            {
                Orecount = 8;
            }
            else if (entity.Level.Difficulty == VanillaDifficulties.lunatic)
            {
                Orecount = 6;
            }
            for (var i = 0; i < Orecount; i++)
            {
                entity.Produce(VanillaPickupID.redstone);
            }

        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);

            Explode(entity, 120, 600);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Remove();

            for (var i = 0; i < 6; i++)
            {
                entity.Spawn(VanillaPickupID.emerald, entity.Position);
                entity.Spawn(VanillaPickupID.ruby, entity.Position);
            }

            for (var i = 0; i < 3; i++)
            {
                entity.Produce(VanillaPickupID.redstone);
            }


        }

        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, VanillaFactions.NEUTRAL, damage, damageEffects, entity);
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
            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);


            return damageOutputs;
        }
    }
}
