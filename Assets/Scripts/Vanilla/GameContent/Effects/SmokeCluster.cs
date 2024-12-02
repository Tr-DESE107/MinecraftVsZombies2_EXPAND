using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.smokeCluster)]
    public class SmokeCluster : EffectBehaviour
    {

        #region 公有方法
        public SmokeCluster(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}