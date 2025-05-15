using MVZ2.GameContent.Enemies;
using PVZEngine.Level;
using MVZ2.GameContent.Buffs.Enemies;
using PVZEngine.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using MVZ2Logic.Level;
using PVZEngine;
using Tools;

namespace MVZ2.Vanilla.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostMutant)]
    public class HostMutant : MutantZombieBase
    {
        public HostMutant(string nsp, string name) : base(nsp, name) { }

        public override void Init(Entity entity)
        {
            base.Init(entity);

            // 添加持续回血 Buff
            var regen = entity.AddBuff<RegenerationBuff>();
            if (regen != null)
            {
                regen.SetProperty(RegenerationBuff.REGEN_HEAL_AMOUNT, 1f);
                regen.SetProperty(RegenerationBuff.REGEN_TIMEOUT, 60000);
            }

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
            if (triggerCount > 0)
            {
                for (int i = 0; i < triggerCount; i++)
                {
                    var randomID = GetRandomSkeletonID(entity.RNG);
                    var enemy = entity.Level.Spawn(randomID, entity.Position, entity);
                    enemy.SetFactionAndDirection(entity.GetFaction());
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
            VanillaEnemyID.HostZombie,

        };

        private static int[] RandomSkeletonWeights = new int[]
        {
            1,
        };

        // 存储“上次触发时的血量”的字段名
        private static readonly VanillaEntityPropertyMeta PROP_LAST_TRIGGER_HEALTH =
            new VanillaEntityPropertyMeta("LastTriggerHealth");

        private static float GetLastTriggerHealth(Entity entity) =>
            entity.GetBehaviourField<float>(ID, PROP_LAST_TRIGGER_HEALTH);

        private static void SetLastTriggerHealth(Entity entity, float hp) =>
            entity.SetBehaviourField(ID, PROP_LAST_TRIGGER_HEALTH, hp);

        // 当前实体的ID（用于字段访问）
        private static readonly NamespaceID ID = VanillaEnemyID.HostMutant;


    }
}
