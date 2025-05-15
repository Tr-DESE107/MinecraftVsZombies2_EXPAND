using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Shells;
using PVZEngine.Damages;
using PVZEngine.Level;
using PVZEngine.Entities;        // 包含 NamespaceID
using MVZ2.Vanilla.Properties;
using PVZEngine;   // 包含 VanillaEntityPropertyMeta



namespace MVZ2.GameContent.Projectiles
{
    [EntityBehaviourDefinition(VanillaProjectileNames.spike)]
    public class Spike : ProjectileBehaviour
    {
        public Spike(string nsp, string name) : base(nsp, name)
        {
        }

        protected override void PostHitEntity(ProjectileHitOutput hitResult, DamageOutput damageOutput)
        {
            base.PostHitEntity(hitResult, damageOutput);
            if (damageOutput == null)
                return;

            var bodyResult = damageOutput.BodyResult;
            var armorResult = damageOutput.ArmorResult;
            var bodyShell = bodyResult?.ShellDefinition;
            var armorShell = armorResult?.ShellDefinition;
            var bodyBlocksSlice = bodyShell != null ? bodyShell.BlocksSlice() : false;
            var armorBlocksSlice = armorShell != null ? armorShell.BlocksSlice() : false;
            var blocksSlice = bodyBlocksSlice || armorBlocksSlice;

            var pierceCount = hitResult.Projectile.GetBehaviourField<int>(ID, PROP_PIERCE_COUNT);

            if (!blocksSlice && pierceCount < MAX_PIERCE)
            {
                hitResult.Pierce = true;
                hitResult.Projectile.SetBehaviourField(ID, PROP_PIERCE_COUNT, pierceCount + 1);
            }
        }

        private const int MAX_PIERCE = 2;
        private static readonly VanillaEntityPropertyMeta PROP_PIERCE_COUNT = new("PierceCount");
        private static readonly NamespaceID ID = VanillaProjectileID.spike;
    }
}
