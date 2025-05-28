using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.spikeParticles)]
    public class SpikeParticles : EffectBehaviour
    {

        #region 公有方法
        public SpikeParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}