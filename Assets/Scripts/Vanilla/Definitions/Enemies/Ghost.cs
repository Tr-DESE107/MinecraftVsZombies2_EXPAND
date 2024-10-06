using MVZ2.Vanilla;
using MVZ2.Vanilla.Buffs;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.ghost)]
    [SpawnDefinition(2)]
    [EntitySeedDefinition(100, VanillaMod.spaceName, VanillaRechargeNames.none)]
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
