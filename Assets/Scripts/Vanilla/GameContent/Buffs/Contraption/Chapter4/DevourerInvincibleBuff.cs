using System;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.Contraption.devourerInvincible)]
    public class DevourerInvincibleBuff : BuffDefinition
    {
        public DevourerInvincibleBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(new NamespaceIDArrayModifier(VanillaEntityProps.GRID_LAYERS, Array.Empty<NamespaceID>()));
        }
    }
}
