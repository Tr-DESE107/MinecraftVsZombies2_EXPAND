using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.parasiteTerror)]
    public class ParasiteTerror : MeleeEnemy
    {
        public ParasiteTerror(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            // 设置血量状态。
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
    }
}
