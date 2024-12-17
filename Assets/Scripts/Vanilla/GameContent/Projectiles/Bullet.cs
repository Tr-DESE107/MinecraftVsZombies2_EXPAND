using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.bullet)]
    public class Bullet : ProjectileBehaviour
    {
        public Bullet(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Remove();
        }
    }
}
