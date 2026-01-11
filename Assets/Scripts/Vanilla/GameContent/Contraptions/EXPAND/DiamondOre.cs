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
using MVZ2.GameContent.Seeds;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.DiamondOre)]
    public class DiamondOre : ContraptionBehaviour
    {
        public DiamondOre(string nsp, string name) : base(nsp, name)
        {

        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            
            for (var i = 0; i < 3; i++)
            {
                entity.Produce(VanillaPickupID.diamond);
            }

        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);

            Explode(entity, 120, 1200);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Remove();


            for (var i = 0; i < 3; i++)
            {
                entity.Produce(VanillaPickupID.diamond);
            }

            //if (entity.RNG.Next(2) == 0) 
            //{ 
            var spawnParams = new SpawnParams();
            var blueprintID = VanillaContraptionID.DiamondOre;
            spawnParams.SetProperty(VanillaPickupProps.CONTENT_ID, blueprintID);
            entity.Produce(VanillaPickupID.blueprintPickup, spawnParams);
            //}


        }

        public static DamageOutput[] Explode(Entity entity, float range, float damage)
        {
            var damageEffects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            var damageOutputs = entity.Level.Explode(entity.Position, range, VanillaFactions.NEUTRAL, damage, damageEffects, entity);
            foreach (var output in damageOutputs)
            {
                var result = output.BodyResult;
                if (result != null && result.Fatal)
                {
                    var target = output.Entity;
                    var distance = (target.Position - entity.Position).magnitude;
                    var speed = 25 * Mathf.Lerp(1f, 0.5f, distance / range);
                    target.Velocity = target.Velocity + Vector3.up * speed;
                }
            }
            Explosion.Spawn(entity, entity.GetCenter(), range);
            entity.PlaySound(VanillaSoundID.explosion);
            entity.Level.ShakeScreen(10, 0, 15);


            return damageOutputs;
        }
    }
}