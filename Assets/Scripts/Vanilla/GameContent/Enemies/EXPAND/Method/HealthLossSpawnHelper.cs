#nullable enable

using System;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Grids;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    /// <summary>  
    /// 玩家模型/宿主突变通用的“掉血达到阈值后生成敌方器械/单位”逻辑。  
    /// 由于这些实体分别继承自 BoatedEnemyBehaviour、Imp、MutantZombieBase 等不同基类，  
    /// 无法用单一基类统一，故抽成静态工具类，各实体只需传入自己的数据即可复用。  
    /// </summary>  
    public static class HealthLossSpawnHelper
    {
        // 所有实体共用同一个属性元数据（名字与旧版本一致，存档兼容）  
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH =
            new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        public static float GetLastTriggerHealth(Entity entity, NamespaceID id) =>
            entity.GetBehaviourField<float>(id, PROP_LAST_TRIGGER_HEALTH);

        public static void SetLastTriggerHealth(Entity entity, NamespaceID id, float hp) =>
            entity.SetBehaviourField(id, PROP_LAST_TRIGGER_HEALTH, hp);

        /// <summary>在 Init 中调用，记录初始血量。</summary>  
        public static void Init(Entity entity, NamespaceID id) =>
            SetLastTriggerHealth(entity, id, entity.Health);

        /// <summary>  
        /// 在 UpdateLogic 中调用。每损失 threshold 点血量触发一次 spawnAction，最多 maxCount 次。  
        /// </summary>  
        public static void CheckHealthLossTrigger(Entity entity, NamespaceID id,
            float threshold, int maxCount, Action<Entity> spawnAction)
        {
            float lastHP = GetLastTriggerHealth(entity, id);
            float currHP = entity.Health;

            int triggerCount = (int)((lastHP - currHP) / threshold);
            triggerCount = Mathf.Min(triggerCount, maxCount);
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                    spawnAction(entity);
                SetLastTriggerHealth(entity, id, currHP);
            }
        }

        // ===== 生成策略 1：内置加权随机池 =====  
        public static void SpawnWeightedRandom(Entity entity, NamespaceID[] pool, int[] weights)
        {
            var index = entity.RNG.WeightedRandom(weights);
            var randomID = pool[index];
            var spawnParam = entity.GetSpawnParams();
            spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
            entity.Spawn(randomID, entity.Position, spawnParam);
        }

        // ===== 生成策略 2：跟随图鉴进度 =====  
        public static void SpawnFromAlmanac(Entity entity)
        {
            var grid = entity.GetGrid();
            if (grid == null)
                return;

            var game = Global.Game;
            var rng = entity.RNG;
            entity.ClearTakenGrids();

            var unlockedContraptions = Global.Saves.GetUnlockedContraptions();
            var validContraptions = unlockedContraptions.Where(id =>
            {
                if (!Global.Almanac.IsContraptionInAlmanac(id))
                    return false;
                var def = game.GetEntityDefinition(id);
                if (def == null || def.IsUpgradeBlueprint())
                    return false;
                return grid.CanSpawnEntity(id);
            });
            if (validContraptions.Count() <= 0)
                return;

            var contraptionID = validContraptions.Random(rng);
            if (contraptionID == VanillaContraptionID.devourer || contraptionID == VanillaContraptionID.jeweledPagoda)
                contraptionID = VanillaContraptionID.dispenser;

            var spawned = entity.SpawnWithParams(contraptionID, entity.Position);
            if (spawned != null && spawned.HasBuff<NocturnalBuff>())
                spawned?.RemoveBuffs<NocturnalBuff>();
        }
    }
}
