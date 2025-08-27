using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.fireworkDispenserEvoked)]
    public class FireworkDispenserEvokedBuff : BuffDefinition
    {
        public FireworkDispenserEvokedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.RANGE, NumberOperator.Add, 80));
            AddModifier(new NamespaceIDModifier(VanillaEntityProps.PROJECTILE_ID, VanillaProjectileID.fireworkBig));
        }
    }
}
