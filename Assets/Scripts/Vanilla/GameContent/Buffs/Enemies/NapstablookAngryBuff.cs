using MVZ2.Vanilla;
using PVZEngine.Buffs;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.napstablookAngry)]
    public class NapstablookAngryBuff : BuffDefinition
    {
        public NapstablookAngryBuff(string nsp, string name) : base(nsp, name)
        {
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
        public const int MAX_TIMEOUT = 30;
    }
}
