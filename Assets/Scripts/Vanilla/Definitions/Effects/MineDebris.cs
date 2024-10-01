using MVZ2.Vanilla;
using PVZEngine.Level;

namespace MVZ2.GameContent.Effects
{
    [Definition(VanillaEffectNames.mineDebris)]
    public class MineDebris : VanillaEffect
    {
        #region 公有方法
        public MineDebris(string nsp, string name) : base(nsp, name)
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