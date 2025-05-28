using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.cursedFireburn)]
    public class CursedFireburn : EffectBehaviour
    {

        #region 公有方法
        public CursedFireburn(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}