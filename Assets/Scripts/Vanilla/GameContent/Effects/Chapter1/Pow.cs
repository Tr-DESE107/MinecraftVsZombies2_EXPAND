using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.pow)]
    public class Pow : EffectBehaviour
    {

        #region 公有方法
        public Pow(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}