using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.pow)]
    public class Pow : EffectBehaviour
    {

        #region 公有方法
        public Pow(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}