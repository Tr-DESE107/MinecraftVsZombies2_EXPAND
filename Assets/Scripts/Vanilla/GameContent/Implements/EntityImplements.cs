using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Implements
{
    public class EntityImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostEntityInit, PostEntityInitCallback);
            mod.RegisterCallback(LevelCallbacks.PostEntityContactGround, PostContactGroundCallback);
            mod.RegisterCallback(VanillaLevelCallbacks.PostEntityTakeDamage, PostEnemyTakeDamageCallback);
        }
        private void PostEntityInitCallback(Entity entity)
        {
            entity.AddBuff<EntityPhysicsBuff>();
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
                source.PlaySound(VanillaSoundID.grind);
            }
        }
    }
}
