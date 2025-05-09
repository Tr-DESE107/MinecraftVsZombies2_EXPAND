using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Carts
{
    [EntityBehaviourDefinition(VanillaCartNames.bowlChariot)]
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
            var scale = new Vector3(0.6666f, 0.6666f, 0.6666f);
            var param = entity.GetSpawnParams();
            param.SetProperty(VanillaProjectileProps.PIERCING, true);
            param.SetProperty(EngineEntityProps.DISPLAY_SCALE, scale);
            param.SetProperty(EngineEntityProps.SCALE, scale);
            param.SetProperty(VanillaEntityProps.DAMAGE, 100);
            var boulder = entity.Spawn(VanillaProjectileID.boulder, entity.Position + new Vector3(0, 16, 0), param);
            boulder.Velocity = Vector3.right * 10;
        }
    }
}