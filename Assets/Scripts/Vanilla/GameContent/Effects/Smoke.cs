using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.smoke)]
    public class Smoke : EffectBehaviour
    {

        #region 公有方法
        public Smoke(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Size", entity.GetScaledSize());
        }
        #endregion
    }
}