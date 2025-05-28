using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.bloodParticles)]
    public class BloodParticles : EffectBehaviour
    {

        #region 公有方法
        public BloodParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}