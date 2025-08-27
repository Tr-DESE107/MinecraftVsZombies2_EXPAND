using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.projectileExplodeMeteor)]
    public class ProjectileExplodeBehaviour_Meteor : ProjectileExplodeBehaviour
    {
        public ProjectileExplodeBehaviour_Meteor(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Explode(Entity entity)
        {
            base.Explode(entity);
            entity.Level.ShakeScreen(10, 0, 15);
        }
        public override void PlayExplosionSound(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.meteorLand);
        }
    }
}
