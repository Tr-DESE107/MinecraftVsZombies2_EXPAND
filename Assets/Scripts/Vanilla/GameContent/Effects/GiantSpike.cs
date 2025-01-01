using MVZ2.GameContent.Damages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.giantSpike)]
    public class GiantSpike : EffectBehaviour
    {
        #region 公有方法
        public GiantSpike(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}