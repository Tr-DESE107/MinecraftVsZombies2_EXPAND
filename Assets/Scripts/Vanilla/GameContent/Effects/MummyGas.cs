using System;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.mummyGas)]
    public class MummyGas : EffectBehaviour
    {

        #region 公有方法
        public MummyGas(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.CollisionMask |=
                EntityCollision.MASK_PLANT |
                EntityCollision.MASK_ENEMY |
                EntityCollision.MASK_BOSS |
                EntityCollision.MASK_OBSTACLE |
                EntityCollision.MASK_PROJECTILE |
                EntityCollision.MASK_EFFECT;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Size", entity.GetScaledSize());
            entity.SetModelProperty("Stopped", entity.Timeout <= DISAPPEAR_TIMEOUT);
        }
        public override void PostCollision(Entity entity, Entity other, int state)
        {
            base.PostCollision(entity, other, state);
            if (state == EntityCollision.STATE_EXIT)
                return;
            if (entity.Timeout <= DISAPPEAR_TIMEOUT)
                return;
            if (other.IsFire())
            {
                entity.PlaySound(VanillaSoundID.fire);
                entity.Timeout = Math.Min(entity.Timeout, DISAPPEAR_TIMEOUT);
                var burning = entity.Level.Spawn(VanillaEffectID.burningGas, entity.Position, entity);
                burning.SetSize(entity.GetSize());

                foreach (var ent in entity.GetCollisionEntities())
                {
                    if (entity.IsHostile(entity))
                        continue;
                    var damageEffectList = new DamageEffectList(VanillaDamageEffects.FIRE);
                    ent.TakeDamage(100, damageEffectList, new EntityReferenceChain(entity));
                }
                return;
            }
            if (other.IsDead)
                return;
            if (other.IsUndead())
            {
                other.HealEffects(entity.GetDamage(), entity);
            }
            else if (entity.IsHostile(other))
            {
                var damageEffects = new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR, VanillaDamageEffects.MUTE);
                other.TakeDamage(entity.GetDamage(), damageEffects, new EntityReferenceChain(entity));
            }
        }
        #endregion

        public const int DISAPPEAR_TIMEOUT = 30;
    }
}