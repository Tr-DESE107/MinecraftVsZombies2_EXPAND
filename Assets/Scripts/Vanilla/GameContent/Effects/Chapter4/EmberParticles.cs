using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.emberParticles)]
    public class EmberParticles : EffectBehaviour
    {

        #region 公有方法
        public EmberParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}