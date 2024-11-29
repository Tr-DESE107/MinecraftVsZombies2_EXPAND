using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.explosion)]
    public class Explosion : EffectBehaviour
    {

        #region 公有方法
        public Explosion(string nsp, string name) : base(nsp, name)
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