using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Level;

namespace MVZ2.Vanilla.Enemies  
{  
    [EntityBehaviourDefinition(VanillaEnemyNames.RandomImp)]  
    public class RandomImp : MeleeEnemy
    {  
        public RandomImp(string nsp, string name) : base(nsp, name)  
        {  
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (entity.Level.IsIZombie())
            {
                entity.AddBuff<IZombieImpBuff>();
            }
        }

        public override void PostDeath(Entity entity, DeathInfo info)  
        {  
            base.PostDeath(entity, info);  
              
            // 检查是否应该移除实体而不生成怪物  
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))  
                return;  
              
            // 生成随机僵尸  
            SpawnRandomEnemy(entity);
            entity.Remove();
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
  
        #region 白名单和权重配置  
          
        // 定义可生成的僵尸白名单  
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]  
        {  
            VanillaEnemyID.imp,  
            VanillaEnemyID.HostIMP,  
            VanillaEnemyID.ImpMannequin,  
            VanillaEnemyID.MegaImpMannequin,  
            
        };  
  
        // 定义每个僵尸的生成权重（数值越大，生成概率越高）  
        private static int[] SpawnWeights = new int[]  
        {  
            10,  // zombie 权重 10  
            5,   // leatherCappedZombie 权重 5  
            3,   // ironHelmettedZombie 权重 2  
            2,   // skeleton 权重 3  
            
        };  
  
        #endregion  
  
        #region 属性字段  
  
        // 当前实体的ID（用于字段访问）  
        private static readonly NamespaceID ID = VanillaEnemyID.RandomImp;  
  
        #endregion  
    }  
}