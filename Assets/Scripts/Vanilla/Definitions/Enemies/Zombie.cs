using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.LevelManagement;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.zombie)]
    [SpawnDefinition(1)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.none)]
    public class Zombie : MeleeEnemy
    {
        public Zombie(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SIZE, new Vector3(32, 86, 32));
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            int healthState;
            float maxHP = entity.GetMaxHealth();
            if (entity.Health > maxHP * 0.5f)
                healthState = 1;
            else
                healthState = 0;
            entity.SetAnimationInt("HealthState", healthState);
        }
    }
}
