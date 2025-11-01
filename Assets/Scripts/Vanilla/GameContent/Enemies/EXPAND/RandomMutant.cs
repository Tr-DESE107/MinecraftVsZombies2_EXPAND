using MVZ2.GameContent.Enemies;
using PVZEngine.Level;
using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using Tools;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.RandomMutant)]
    public class RandomMutant : MutantZombieBase
    {
        public RandomMutant(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaEnemyID.RandomImp);
        }

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);

            // 检查是否应该移除实体而不生成怪物  
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            // 生成随机僵尸  
            SpawnRandomEnemy(entity);
        }

        /// <summary>  
        /// 在原位置生成随机僵尸  
        /// </summary>  
        private void SpawnRandomEnemy(Entity entity)
        {
            var level = entity.Level;
            var rng = entity.RNG;

            // 使用白名单和权重生成  
            NamespaceID enemyID = GetRandomEnemyID(rng);

            // 在原位置生成僵尸  
            var spawnParam = entity.GetSpawnParams();
            spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
            entity.Spawn(enemyID, entity.Position, spawnParam);
        }

        /// <summary>  
        /// 根据权重随机选择一个僵尸ID  
        /// </summary>  
        private NamespaceID GetRandomEnemyID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(SpawnWeights);
            return SpawnWhitelist[index];
        }

          

        // 定义可生成的僵尸白名单  
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]
        {
            VanillaEnemyID.mutantZombie,
            VanillaEnemyID.megaMutantZombie,
            VanillaEnemyID.HostMutant,
            VanillaEnemyID.HeavyGutant,
            VanillaEnemyID.MegaMutantVillager,
            VanillaEnemyID.MutantVillager,
            VanillaEnemyID.MutantMannequin,

        };

        // 定义每个僵尸的生成权重（数值越大，生成概率越高）  
        private static int[] SpawnWeights = new int[]
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            
        };

        private static readonly NamespaceID ID = VanillaEnemyID.RandomMutant;
    }
}