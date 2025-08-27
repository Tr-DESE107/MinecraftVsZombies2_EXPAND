using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.projectileExplodeFireworkBig)]
    public class ProjectileExplodeBehaviour_FireworkBig : ProjectileExplodeBehaviour_Firework
    {
        public ProjectileExplodeBehaviour_FireworkBig(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PlayExplosionSound(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.fireworkLargeblast);
            entity.PlaySound(VanillaSoundID.fireworkTwinkle);
        }
    }
}
