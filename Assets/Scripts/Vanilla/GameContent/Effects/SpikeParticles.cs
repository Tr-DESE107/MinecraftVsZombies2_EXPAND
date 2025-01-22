using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.spikeParticles)]
    public class SpikeParticles : EffectBehaviour
    {

        #region 公有方法
        public SpikeParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}