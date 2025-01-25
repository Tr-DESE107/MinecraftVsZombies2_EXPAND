using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.waterLightningParticles)]
    public class WaterLightningParticles : EffectBehaviour
    {
        #region 公有方法
        public WaterLightningParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}