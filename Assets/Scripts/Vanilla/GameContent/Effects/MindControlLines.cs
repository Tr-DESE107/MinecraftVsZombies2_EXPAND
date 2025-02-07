using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.mindControlLines)]
    public class MindControlLines : EffectBehaviour
    {

        #region 公有方法
        public MindControlLines(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}