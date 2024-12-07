using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.bloodParticles)]
    public class BloodParticles : EffectBehaviour
    {

        #region 公有方法
        public BloodParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}