using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.sliceSpark)]
    public class SliceSpark : EffectBehaviour
    {

        #region 公有方法
        public SliceSpark(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}