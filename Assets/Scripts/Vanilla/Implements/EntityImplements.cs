using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Modding;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class EntityImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostEntityContactGround, PostContactGroundCallback);
        }
        private void PostContactGroundCallback(Entity entity, Vector3 velocity)
        {
            if (!EntityTypes.IsDamagable(entity.Type))
                return;
            float fallHeight = Mathf.Max(0, entity.GetFallDamage() - velocity.y * 5);
            float fallDamage = Mathf.Pow(fallHeight, 2);
            if (fallDamage > 0)
            {
                var effects = new DamageEffectList(EngineDamageEffects.IGNORE_ARMOR, EngineDamageEffects.FALL_DAMAGE);
                entity.TakeDamage(fallDamage, effects, new EntityReferenceChain(null));
            }
        }
    }
}
