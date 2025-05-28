using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.gearParticles)]
    public class GearParticles : EffectBehaviour
    {

        #region 公有方法
        public GearParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}