using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.chargedBolt)]
    public class ChargedBolt : ProjectileBehaviour
    {
        public ChargedBolt(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Level.AddLoopSoundEntity(VanillaSoundID.chargedBolt, entity.ID);
            entity.SetAnimationFloat("Speed", entity.RNG.NextFloat() * 1.5f + 0.5f);
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            var vel = projectile.Velocity;
            vel.z = projectile.RNG.Next(-1, 2) * 5;
            projectile.Velocity = vel;
        }
    }
}
