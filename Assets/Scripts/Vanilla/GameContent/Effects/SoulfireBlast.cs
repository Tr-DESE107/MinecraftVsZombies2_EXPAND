using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.soulfireBlast)]
    public class SoulfireBlast : EffectBehaviour
    {

        #region 公有方法
        public SoulfireBlast(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 30;
        }
        #endregion
    }
}