﻿using System.Linq;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;
using PVZEngine.Level;

namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.knife)]
    public class Knife : ProjectileBehaviour
    {
        public Knife(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damageOutput)
        {
            base.PostHitEntity(hitResult, damageOutput);
            if (damageOutput == null)
                return;
            var blocksSlice = damageOutput.GetAllResults().Any(e => e?.ShellDefinition?.BlocksSlice() ?? false);
            if (!blocksSlice)
            {
                hitResult.Pierce = true;
            }
        }
    }
}
