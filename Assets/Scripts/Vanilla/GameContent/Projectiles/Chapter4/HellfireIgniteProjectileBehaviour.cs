using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Projectiles;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.hellfireIgnitedProjectile)]
    public class HellfireIgnitedProjectileBehaviour : ProjectileBehaviour
    {
        public HellfireIgnitedProjectileBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity projectile)
        {
            base.Update(projectile);
            var ignited = 0;
            ignitedBuffBuffer.Clear();
            projectile.GetBuffs<HellfireIgnitedBuff>(ignitedBuffBuffer);
            if (ignitedBuffBuffer.Count > 0)
            {
                if (ignitedBuffBuffer.Any(b => HellfireIgnitedBuff.GetCursed(b)))
                {
                    ignited = 2;
                }
                else
                {
                    ignited = 1;
                }
            }
            projectile.SetAnimationInt("Ignited", ignited);
        }
        private List<Buff> ignitedBuffBuffer = new List<Buff>();
    }
}
