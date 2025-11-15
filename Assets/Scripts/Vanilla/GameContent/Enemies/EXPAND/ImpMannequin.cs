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
using MVZ2.GameContent.Damages;

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
            // 初始化记录血量（用于掉血检测）
            SetLastTriggerHealth(entity, entity.Health);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());

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
            VanillaContraptionID.dispenser,
            VanillaContraptionID.drivenser,
            VanillaContraptionID.silvenser,
            VanillaContraptionID.furnace,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            2,
            2,
            4,
            1,
        };

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // 掉落碎船掉落物
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity)?.Let(e =>
                {
                    e.Velocity = new Vector3(e.RNG.NextFloat() * 20 - 10, 5, 0);
                    e.ChangeModel(VanillaModelID.boatItem);
                    e.SetDisplayScale(entity.GetDisplayScale());
                });
            }
            if (info.HasEffect(VanillaDamageEffects.NO_DEATH_TRIGGER))
                return;
            entity.SpawnWithParams(VanillaEnemyID.MannequinTNT, entity.Position);
        }

        // 存储“上次触发时的血量”的字段名
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // 当前实体的ID（用于字段访问）
        private static readonly NamespaceID ID = VanillaEnemyID.ImpMannequin;
    }
}
