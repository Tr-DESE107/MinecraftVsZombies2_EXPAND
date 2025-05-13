using MVZ2.Vanilla.Entities;
using PVZEngine.Level;
using MVZ2.GameContent.Shells;
using MVZ2.Vanilla.Entities;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.Poisonball)]
    public class Poisonball : ProjectileBehaviour
    {
        public Poisonball(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damage)
        {
            base.PostHitEntity(hitResult, damage);
            var enemy = hitResult.Other;
            if (enemy.Type != EntityTypes.ENEMY)
                return;
            /*
            if (damage.BodyResult == null)
                return;
            if (enemy.GetShellID() != VanillaShellID.flesh)
                return;
            */
            enemy.InflictCorropoisonBuff(-1f,150);
        }
    }
}
