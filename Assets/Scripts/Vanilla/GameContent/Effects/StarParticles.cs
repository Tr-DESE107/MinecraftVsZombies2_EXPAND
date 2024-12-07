using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.starParticles)]
    public class StarParticles : EffectBehaviour
    {

        #region 公有方法
        public StarParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}