using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.seijaFaintEffect)]
    public class SeijaFaintEffect : EffectBehaviour
    {

        #region 公有方法
        public SeijaFaintEffect(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}