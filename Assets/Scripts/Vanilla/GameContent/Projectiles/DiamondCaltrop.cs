using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.diamondCaltrop)]
    public class DiamondCaltrop : ProjectileBehaviour
    {
        public DiamondCaltrop(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            var vel = entity.Velocity;
            vel.y = -velocity.y * 0.2f;
            entity.Velocity = vel;
        }
    }
}
