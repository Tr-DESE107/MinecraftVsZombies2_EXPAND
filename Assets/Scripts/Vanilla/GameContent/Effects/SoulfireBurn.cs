using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.soulfireBurn)]
    public class SoulfireBurn : EffectBehaviour
    {

        #region 公有方法
        public SoulfireBurn(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 45;
        }
        #endregion
    }
}