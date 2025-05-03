using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.splashParticles)]
    public class SplashParticles : EffectBehaviour
    {
        #region 公有方法
        public SplashParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}