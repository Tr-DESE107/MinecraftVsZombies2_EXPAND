using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.cursedMeteor)]
    public class CursedMeteor : EffectBehaviour
    {

        #region 公有方法
        public CursedMeteor(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion

        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var parent = entity.Parent;
            if (parent.ExistsAndAlive() && parent.IsEntityOf(VanillaContraptionID.hellfire))
            {
                Hellfire.Curse(parent);
            }
            var range = entity.GetRange();
            var effects = new DamageEffectList(VanillaDamageEffects.MUTE, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.EXPLOSION);
            entity.Level.Explode(entity.GetCenter(), range, entity.GetFaction(), entity.GetDamage(), effects, entity);


            var param = entity.GetSpawnParams();
            param.SetProperty(EngineEntityProps.SIZE, Vector3.one * (range * 2));
            var explosion = entity.Spawn(VanillaEffectID.explosion, entity.GetCenter(), param);

            entity.Spawn(VanillaEffectID.cursedFireParticles, entity.GetCenter());

            entity.PlaySound(VanillaSoundID.meteorLand);
            entity.Level.ShakeScreen(10, 0, 15);
            entity.Remove();
        }
    }
}