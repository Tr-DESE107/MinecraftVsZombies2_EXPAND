using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Effects
{
    [Definition(EffectNames.mineDebris)]
    public class MineDebris : VanillaEffect
    {
        #region 公有方法
        public override void Init(Entity entity)
        {
            entity.Timeout = 30;
        }
        public override void Update(Entity entity)
        {
            entity.Timeout--;
            if (entity.Timeout <= 0)
            {
                entity.Remove();
            }
        }
        #endregion
    }
}