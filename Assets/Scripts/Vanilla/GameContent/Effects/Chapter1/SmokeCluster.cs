using MVZ2.Vanilla.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.smokeCluster)]
    public class SmokeCluster : EffectBehaviour
    {

        #region 公有方法
        public SmokeCluster(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}