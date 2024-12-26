using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.boneParticles)]
    public class BoneParticles : EffectBehaviour
    {

        #region 公有方法
        public BoneParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}