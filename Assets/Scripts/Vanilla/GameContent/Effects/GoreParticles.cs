using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.goreParticles)]
    public class GoreParticles : EffectBehaviour
    {

        #region 公有方法
        public GoreParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}