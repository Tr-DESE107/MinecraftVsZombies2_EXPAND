using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.berserker)]
    public class Berserker : MeleeEnemy
    {
        public Berserker(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipMainArmor(VanillaArmorID.bersekerHelmet);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
            if (info.Effects.HasEffect(VanillaDamageEffects.DROWN))
                return;
            Explode(entity, entity.GetDamage() * 3, entity.GetFaction());
            entity.Remove();
        }
        public static void Explode(Entity entity, float damage, int faction)
        {
            var scale = entity.GetFinalScale();
            var scaleX = Mathf.Abs(scale.x);
            var range = entity.GetRange() * scaleX;
            entity.Explode(entity.GetCenter(), range, faction, damage, new DamageEffectList(VanillaDamageEffects.EXPLOSION, VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN, VanillaDamageEffects.MUTE));

            var explosion = entity.Level.Spawn(VanillaEffectID.explosion, entity.GetCenter(), entity);
            explosion.SetSize(Vector3.one * (range * 2));
            entity.PlaySound(VanillaSoundID.explosion, scaleX == 0 ? 1000 : 1 / (scaleX));
        }
    }
}
