using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.cursedFireParticles)]
    public class CursedFireParticles : EffectBehaviour
    {

        #region 公有方法
        public CursedFireParticles(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}