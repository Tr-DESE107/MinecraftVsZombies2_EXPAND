using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.swordParalyzed)]
    public class SwordParalyzedBuff : BuffDefinition
    {
        public SwordParalyzedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaLevelProps.KEEP_HELD_ITEM_IN_SCREEN, true));
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
        public override void PostRemove(Buff buff)
        {
            base.PostRemove(buff);
        }
        public const string PROP_TIMEOUT = "Timeout";
        public const int MAX_TIMEOUT = 45;
    }
}
