using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.gearParticles)]
    public class GearParticles : EffectBehaviour
    {

        #region 公有方法
        public GearParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}