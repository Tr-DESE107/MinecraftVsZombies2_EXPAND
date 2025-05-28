using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.waterLightningParticles)]
    public class WaterLightningParticles : EffectBehaviour
    {
        #region 公有方法
        public WaterLightningParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}