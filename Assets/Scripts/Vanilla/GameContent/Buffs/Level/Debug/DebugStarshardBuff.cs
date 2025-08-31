using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.debugStarshard)]
    public class DebugStarshardBuff : BuffDefinition
    {
        public DebugStarshardBuff(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            buff.Level.AddStarshardCount(5);
        }
    }
}
