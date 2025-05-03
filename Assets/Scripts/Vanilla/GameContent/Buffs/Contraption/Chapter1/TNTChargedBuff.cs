using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Models;
using MVZ2Logic.Models;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.tntCharged)]
    public class TNTChargedBuff : BuffDefinition
    {
        public TNTChargedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModelInsertion(LogicModelHelper.ANCHOR_CENTER, VanillaModelKeys.staticParticles, VanillaModelID.staticParticles);
        }
    }
}
