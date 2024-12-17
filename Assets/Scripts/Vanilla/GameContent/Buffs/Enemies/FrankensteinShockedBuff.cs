using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.frankensteinShocked)]
    public class FrankensteinShockedBuff : BuffDefinition
    {
        public FrankensteinShockedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.AI_FROZEN, true));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateShocked(buff);
        }
        private void UpdateShocked(Buff buff)
        {
            var timeout = buff.GetProperty<float>(PROP_TIMEOUT);
            timeout--;
            buff.SetProperty(PROP_TIMEOUT, timeout);
            if (timeout <= 0)
            {
                buff.Remove();
                return;
            }
            var entity = buff.GetEntity();
            if (entity == null || !entity.Exists() || entity.IsDead)
            {
                buff.Remove();
                return;
            }
        }
        public const string PROP_TIMEOUT = "Timeout";
    }
}
