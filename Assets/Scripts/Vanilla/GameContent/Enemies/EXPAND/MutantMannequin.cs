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

            // 初始化记录血量（用于掉血检测）
            SetLastTriggerHealth(entity, entity.Health);
        }

        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            // 检查是否需要触发血量下降事件
            CheckHealthLossTrigger(entity);
        }

        /// <summary>
        /// 每当血量下降超过设定阈值，触发一次事件
        /// </summary>
        private void CheckHealthLossTrigger(Entity entity)
        {
            float lastHP = GetLastTriggerHealth(entity);
            float currHP = entity.Health;

            // 每掉落一定血量（例如 800 点）触发一次事件
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

                // 更新记录的血量
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
            //怪物出怪
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

        // 存储“上次触发时的血量”的字段名
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // 当前实体的ID（用于字段访问）
        private static readonly NamespaceID ID = VanillaEnemyID.MutantMannequin;
    }
}
