using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using Tools;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.Mannequin)]
    public class Mannequin : MeleeEnemy
    {
        public Mannequin(string nsp, string name) : base(nsp, name)
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
            entity.SetModelDamagePercent();
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());

            // 检查是否需要触发血量下降事件
            CheckHealthLossTrigger(entity);
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // 掉落碎船掉落物
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity);
                effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                effect.ChangeModel(VanillaModelID.boatItem);
                effect.SetDisplayScale(entity.GetDisplayScale());
            }

            entity.SpawnWithParams(VanillaContraptionID.tnt, entity.Position);
        }




        /// <summary>
        /// 每当血量下降超过设定阈值，触发一次事件
        /// </summary>
        private void CheckHealthLossTrigger(Entity entity)
        {
            float lastHP = GetLastTriggerHealth(entity);
            float currHP = entity.Health;

            // 每掉落一定血量（例如 100 点）触发一次事件
            int triggerCount = (int)((lastHP - currHP) / 100f);
            triggerCount = Mathf.Min(triggerCount, 2);
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                {
                    var grid = entity.GetGrid();
                    if (grid == null)
                        return;

                    var game = Global.Game;
                    var level = entity.Level;
                    var rng = entity.RNG;
                    entity.ClearTakenGrids();
                    var unlockedContraptions = game.GetUnlockedContraptions();
                    var validContraptions = unlockedContraptions.Where(id =>
                    {
                        if (!game.IsContraptionInAlmanac(id))
                            return false;
                        var def = game.GetEntityDefinition(id);
                        if (def.IsUpgradeBlueprint())
                            return false;
                        return grid.CanSpawnEntity(id);
                    });
                    if (validContraptions.Count() <= 0)
                        return;
                    var contraptionID = validContraptions.Random(rng);
                    if (contraptionID == VanillaContraptionID.devourer)
                    {
                        contraptionID = VanillaContraptionID.dispenser;
                    }
                    var spawned = entity.SpawnWithParams(contraptionID, entity.Position);
                    if (spawned != null && spawned.HasBuff<NocturnalBuff>())
                    {
                        spawned.RemoveBuffs<NocturnalBuff>();
                    }
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

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            1,
        };

        // 存储“上次触发时的血量”的字段名
        private static readonly VanillaEntityPropertyMeta<float> PROP_LAST_TRIGGER_HEALTH = new VanillaEntityPropertyMeta<float>("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // 当前实体的ID（用于字段访问）
        private static readonly NamespaceID ID = VanillaEnemyID.Mannequin;

    }
}
