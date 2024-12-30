using MVZ2.GameContent.Models;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Models;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.dreamButterflyShield)]
    public class DreamButterflyShieldBuff : BuffDefinition
    {
        public DreamButterflyShieldBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.dreamKeyShield, VanillaModelID.dreamKeyShield);
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
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
        public const string PROP_TIMEOUT = "Timeout";
        public const int MAX_TIMEOUT = 90;
    }
}
