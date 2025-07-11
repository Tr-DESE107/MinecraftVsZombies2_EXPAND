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
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (info.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
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

            Explosion.Spawn(entity, entity.GetCenter(), range);

            entity.PlaySound(VanillaSoundID.explosion, scaleX == 0 ? 1000 : 1 / (scaleX));
        }
    }
}
