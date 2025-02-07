using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareaperShadow)]
    public class NightmareaperShadow : EffectBehaviour
    {
        #region 公有方法
        public NightmareaperShadow(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var tint = entity.GetTint();
            tint.a = entity.Timeout / (float)entity.GetMaxTimeout();
            entity.SetTint(tint);
        }
        #endregion
    }
}