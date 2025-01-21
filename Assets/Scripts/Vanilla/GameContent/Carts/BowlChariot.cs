using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Carts
{
    [Definition(VanillaCartNames.bowlChariot)]
    public class BowlChariot : CartBehaviour
    {
        public BowlChariot(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetAnimationBool("Running", entity.IsCartTriggered());
        }
        public override void PostTrigger(Entity entity)
        {
            base.PostTrigger(entity);
            var boulder = entity.Spawn(VanillaProjectileID.boulder, entity.Position + new Vector3(0, 16, 0));
            boulder.Velocity = Vector3.right * 10;
            boulder.SetPiercing(true);
            var scale = new Vector3(0.6666f, 0.6666f, 0.6666f);
            boulder.SetDisplayScale(scale);
            boulder.SetScale(scale);
            boulder.SetDamage(100);
        }
    }
}