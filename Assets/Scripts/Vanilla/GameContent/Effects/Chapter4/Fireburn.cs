using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.fireburn)]
    public class Fireburn : EffectBehaviour
    {

        #region 公有方法
        public Fireburn(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}