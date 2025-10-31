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
              
            // ����Ƿ�Ӧ���Ƴ�ʵ��������ɹ���  
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))  
                return;  
              
            // ���������ʬ  
            SpawnRandomEnemy(entity);  
        }  
  
        /// <summary>  
        /// ��ԭλ�����������ʬ  
        /// </summary>  
        private void SpawnRandomEnemy(Entity entity)  
        {  
            var level = entity.Level;  
            var rng = entity.RNG;  
              
            // ʹ�ð�������Ȩ������  
            NamespaceID enemyID = GetRandomEnemyID(rng);  
              
            // ��ԭλ�����ɽ�ʬ  
            var spawnParam = entity.GetSpawnParams();  
            spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());  
            entity.Spawn(enemyID, entity.Position, spawnParam);  
        }  
  
        /// <summary>  
        /// ����Ȩ�����ѡ��һ����ʬID  
        /// </summary>  
        private NamespaceID GetRandomEnemyID(RandomGenerator rng)  
        {  
            var index = rng.WeightedRandom(SpawnWeights);  
            return SpawnWhitelist[index];  
        }  
  
        #region ��������Ȩ������  
          
        // ��������ɵĽ�ʬ������  
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]  
        {  
            VanillaEnemyID.imp,  
            VanillaEnemyID.HostIMP,  
            VanillaEnemyID.ImpMannequin,  
            VanillaEnemyID.MegaImpMannequin,  
            
        };  
  
        // ����ÿ����ʬ������Ȩ�أ���ֵԽ�����ɸ���Խ�ߣ�  
        private static int[] SpawnWeights = new int[]  
        {  
            10,  // zombie Ȩ�� 10  
            5,   // leatherCappedZombie Ȩ�� 5  
            3,   // ironHelmettedZombie Ȩ�� 2  
            2,   // skeleton Ȩ�� 3  
            
        };  
  
        #endregion  
  
        #region �����ֶ�  
  
        // ��ǰʵ���ID�������ֶη��ʣ�  
        private static readonly NamespaceID ID = VanillaEnemyID.RandomImp;  
  
        #endregion  
    }  
}