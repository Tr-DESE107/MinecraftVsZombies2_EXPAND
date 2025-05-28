using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.soulfire)]
    public class Soulfire : EffectBehaviour
    {

        #region 公有方法
        public Soulfire(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}