using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.soulfire)]
    public class Soulfire : EffectBehaviour
    {

        #region 公有方法
        public Soulfire(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}