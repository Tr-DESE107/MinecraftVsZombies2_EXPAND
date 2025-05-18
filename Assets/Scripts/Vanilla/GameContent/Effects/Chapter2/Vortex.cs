using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.vortex)]
    public class Vortex : EffectBehaviour
    {
        #region 公有方法
        public Vortex(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}