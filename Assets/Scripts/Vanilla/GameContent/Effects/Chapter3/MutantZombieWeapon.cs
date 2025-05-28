using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.mutantZombieWeapon)]
    public class MutantZombieWeapon : EffectBehaviour
    {
        #region ���з���
        public MutantZombieWeapon(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetTint(new Color(1, 1, 1, Mathf.Clamp01(entity.Timeout / 15f)));
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var vel = entity.Velocity;
            vel.y *= -0.4f;
            entity.Velocity = vel;
        }
        #endregion
    }
}