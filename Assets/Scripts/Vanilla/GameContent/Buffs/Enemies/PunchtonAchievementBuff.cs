using MVZ2.Vanilla;
using PVZEngine.Buffs;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.punchtonAchievement)]
    public class PunchtonAchievementBuff : BuffDefinition
    {
        public PunchtonAchievementBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIMEOUT, 10);
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
    }
}
