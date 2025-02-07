using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.diamondSpikeParticles)]
    public class DiamondSpikeParticles : EffectBehaviour
    {

        #region 公有方法
        public DiamondSpikeParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}