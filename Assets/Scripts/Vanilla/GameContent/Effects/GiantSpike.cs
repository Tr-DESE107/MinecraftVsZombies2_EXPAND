using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.giantSpike)]
    public class GiantSpike : EffectBehaviour
    {
        #region 公有方法
        public GiantSpike(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}