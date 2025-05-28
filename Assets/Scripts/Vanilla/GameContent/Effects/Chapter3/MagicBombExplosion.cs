using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.magicBombExplosion)]
    public class MagicBombExplosion : EffectBehaviour
    {
        #region 公有方法
        public MagicBombExplosion(string nsp, string name) : base(nsp, name)
        {
        }
        #endregion
    }
}