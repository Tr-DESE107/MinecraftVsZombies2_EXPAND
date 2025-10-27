using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using Tools;
using MVZ2.GameContent.Buffs.Contraptions;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.MutantMannequin)]
    public class MutantMannequin : MutantZombieBase
    {
        public MutantMannequin(string nsp, string name) : base(nsp, name)
        {
            SetImpID(VanillaEnemyID.MegaImpMannequin);
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            // ��ʼ����¼Ѫ�������ڵ�Ѫ��⣩
            SetLastTriggerHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            // ����Ƿ���Ҫ����Ѫ���½��¼�
            CheckHealthLossTrigger(entity);
        }

        /// <summary>
        /// ÿ��Ѫ���½������趨��ֵ������һ���¼�
        /// </summary>
        private void CheckHealthLossTrigger(Entity entity)
        {
            float lastHP = GetLastTriggerHealth(entity);
            float currHP = entity.Health;

            // ÿ����һ��Ѫ�������� 800 �㣩����һ���¼�
            int triggerCount = (int)((lastHP - currHP) / 800f);
            triggerCount = Mathf.Min(triggerCount, 2);
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                {
                    var randomID = GetRandomSkeletonID(entity.RNG);
                    var spawnParam = entity.GetSpawnParams();
                    spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
                    entity.Spawn(randomID, entity.Position, spawnParam);
                }

                // ���¼�¼��Ѫ��
                SetLastTriggerHealth(entity, currHP);
            }
        }

        public NamespaceID GetRandomSkeletonID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(RandomSkeletonWeights);
            return RandomSkeleton[index];
        }

        private static NamespaceID[] RandomSkeleton = new NamespaceID[]
        {
            //�������
            VanillaContraptionID.DispenShield,
            VanillaContraptionID.soulFurnace,
            VanillaContraptionID.necrotombstone,
            VanillaContraptionID.totenser,
            VanillaContraptionID.silvenser,
            VanillaContraptionID.CopperDropper,
            VanillaContraptionID.stoneDropper,
            VanillaContraptionID.furnace,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            5,
            5,
            5,
            5,
            10,
            1,
            10,
            8,
        };

        // �洢���ϴδ���ʱ��Ѫ�������ֶ���
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // ��ǰʵ���ID�������ֶη��ʣ�
        private static readonly NamespaceID ID = VanillaEnemyID.MutantMannequin;
    }
}
