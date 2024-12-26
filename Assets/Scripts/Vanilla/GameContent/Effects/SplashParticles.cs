using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.splashParticles)]
    public class SplashParticles : EffectBehaviour
    {
        #region 公有方法
        public SplashParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}