﻿using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.nightmareDecrepify)]
    public class NightmareDecrepifyBuff : BuffDefinition
    {
        public NightmareDecrepifyBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new NamespaceIDModifier(VanillaLevelProps.PICKAXE_DISABLE_ID, VanillaBlueprintErrors.decrepify));
            AddModifier(new BooleanModifier(VanillaLevelProps.PICKAXE_DISABLE_ICON, true));

            AddModifier(new NamespaceIDModifier(VanillaLevelProps.STARSHARD_DISABLE_ID, VanillaBlueprintErrors.decrepify));
            AddModifier(new BooleanModifier(VanillaLevelProps.STARSHARD_DISABLE_ICON, true));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, MAX_TIMEOUT);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var timeout = buff.GetProperty<int>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
            }
        }
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIMEOUT = new VanillaBuffPropertyMeta<int>("Timeout");
        public const int MAX_TIMEOUT = 1800;
    }
}
