using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.mineDebris)]
    public class MineDebris : EffectBehaviour
    {
        #region 公有方法
        public MineDebris(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}