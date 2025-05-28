using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.burningGas)]
    public class BurningGas : EffectBehaviour
    {
        #region 公有方法
        public BurningGas(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            entity.SetModelProperty("Size", entity.GetScaledSize());
            entity.SetModelProperty("Timeout", entity.Timeout);
        }
        #endregion
    }
}