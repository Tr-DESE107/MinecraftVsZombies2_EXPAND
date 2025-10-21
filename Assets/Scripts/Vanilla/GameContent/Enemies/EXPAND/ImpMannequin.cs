using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.ImpMannequin)]
    public class ImpMannequin : AIEntityBehaviour
    {
        public ImpMannequin(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (entity.Level.IsIZombie())
            {
                entity.AddBuff<IZombieImpBuff>();
            }
            // ��ʼ����¼Ѫ�������ڵ�Ѫ��⣩
            SetLastTriggerHealth(entity, entity.Health);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());

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
            int triggerCount = (int)((lastHP - currHP) / 100f);
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
            VanillaContraptionID.dispenser,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            1,
        };

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // �����鴬������
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity)?.Let(e =>
                {
                    e.Velocity = new Vector3(e.RNG.NextFloat() * 20 - 10, 5, 0);
                    e.ChangeModel(VanillaModelID.boatItem);
                    e.SetDisplayScale(entity.GetDisplayScale());
                });
            }

            entity.SpawnWithParams(VanillaContraptionID.tnt, entity.Position);
        }

        // �洢���ϴδ���ʱ��Ѫ�������ֶ���
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // ��ǰʵ���ID�������ֶη��ʣ�
        private static readonly NamespaceID ID = VanillaEnemyID.ImpMannequin;
    }
}
