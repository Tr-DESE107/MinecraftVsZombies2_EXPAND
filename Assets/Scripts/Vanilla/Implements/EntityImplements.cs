using MVZ2Logic.Level;
using MVZ2.GameContent;
using MVZ2Logic.Audios;
using MVZ2Logic.Entities;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class EntityImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostEntityContactGround, PostContactGroundCallback);
            mod.RegisterCallback(VanillaLevelCallbacks.PostEntityTakeDamage, PostEnemyTakeDamageCallback);
        }
        private void PostContactGroundCallback(Entity entity, Vector3 velocity)
        {
            if (!EntityTypes.IsDamagable(entity.Type))
                return;
            float fallHeight = Mathf.Max(0, entity.GetFallDamage() - velocity.y * 5);
            float fallDamage = Mathf.Pow(fallHeight, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.FALL_DAMAGE);
                entity.TakeDamage(fallDamage, effects, new EntityReferenceChain(null));
            }
        }
        private void PostEnemyTakeDamageCallback(DamageResult bodyResult, DamageResult armorResult)
        {
            if (bodyResult == null)
                return;
            var entity = bodyResult.Entity;
            var source = bodyResult.Source?.GetEntity(entity.Level);
            if (source == null)
                return;
            if (bodyResult.Fatal && source.IsEntityOf(VanillaProjectileID.largeSnowball))
            {
                source.PlaySound(SoundID.grind);
            }
        }
    }
}
