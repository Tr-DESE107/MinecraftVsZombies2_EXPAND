using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.smoke)]
    public class Smoke : VanillaEffect
    {

        #region 公有方法
        public Smoke(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = 30;
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Size", entity.GetScaledSize());
        }
        #endregion
    }
}