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

          

        // ��������ɵĽ�ʬ������  
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

        // ����ÿ����ʬ������Ȩ�أ���ֵԽ�����ɸ���Խ�ߣ�  
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