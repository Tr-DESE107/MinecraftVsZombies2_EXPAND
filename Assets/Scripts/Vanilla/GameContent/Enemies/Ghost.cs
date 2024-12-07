using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.ghost)]
    public class Ghost : MeleeEnemy
    {
        public Ghost(string nsp, string name) : base(nsp, name)
        {
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
            if (!entity.HasBuff<GhostBuff>())
            {
                entity.AddBuff<GhostBuff>();
            }
        }
    }
}
