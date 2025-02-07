using MVZ2.Vanilla;
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
            AddModifier(new BooleanModifier(VanillaLevelProps.PICKAXE_DISABLED, true));
            AddModifier(new StringModifier(VanillaLevelProps.PICKAXE_DISABLE_MESSAGE, VanillaStrings.TOOLTIP_DECREPIFY));
            AddModifier(new BooleanModifier(VanillaLevelProps.PICKAXE_DISABLE_ICON, true));
            AddModifier(new BooleanModifier(VanillaLevelProps.STARSHARD_DISABLED, true));
            AddModifier(new StringModifier(VanillaLevelProps.STARSHARD_DISABLE_MESSAGE, VanillaStrings.TOOLTIP_DECREPIFY));
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
        public static readonly VanillaBuffPropertyMeta PROP_TIMEOUT = new VanillaBuffPropertyMeta("Timeout");
        public const int MAX_TIMEOUT = 1800;
    }
}
