using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using PVZEngine.Armors;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Projectiles
{
    [Definition(VanillaProjectileNames.knife)]
    public class Knife : ProjectileBehaviour
    {
        public Knife(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void PostHitEntity(ProjectileHitResult hitResult, DamageResult bodyResult, DamageResult armorResult)
        {
            base.PostHitEntity(hitResult, bodyResult, armorResult);
            var bodyShell = bodyResult?.ShellDefinition;
            var armorShell = armorResult?.ShellDefinition;
            var bodyBlocksSlice = bodyShell != null ? bodyShell.BlocksSlice() : false;
            var armorBlocksSlice = armorShell != null ? armorShell.BlocksSlice() : false;
            var blocksSlice = bodyBlocksSlice || armorBlocksSlice;
            if (!blocksSlice)
            {
                hitResult.Pierce = true;
            }
        }
    }
}
