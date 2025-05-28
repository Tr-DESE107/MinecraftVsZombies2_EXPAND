using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareaperSplash)]
    public class NightmareaperSplash : EffectBehaviour
    {

        #region 公有方法
        public NightmareaperSplash(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.SetTint(entity.Level.GetWaterColor());
        }
        #endregion
    }
}