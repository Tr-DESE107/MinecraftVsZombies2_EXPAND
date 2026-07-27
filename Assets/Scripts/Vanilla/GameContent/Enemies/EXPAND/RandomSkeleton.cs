#nullable enable

using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Entities;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Entities;
using MVZ2.Vanilla.Projectiles;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.RandomSkeleton)]
    public class RandomSkeleton : AIEntityBehaviour
    {
        public RandomSkeleton(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true,
                ignoreLowEnemy = true
            };
        }

        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);

            // 寻找目标  
            if (CanShoot(enemy))
            {
                if (enemy.Target != null && !ValidateTarget(enemy, enemy.Target))
                {
                    enemy.Target = null;
                }
                enemy.Target = FindTarget(enemy);
            }
            else
            {
                enemy.Target = null;
            }

            // 根据状态执行AI行为  
            switch (enemy.State)
            {
                case STATE_RANGED_ATTACK:
                    PullBow(enemy);
                    break;
                default:
                    UnleaseBow(enemy);
                    break;
            }
        }

        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationFloat("BowBlend", 1 - Mathf.Pow(1 - GetBowPower(entity) / (float)BOW_POWER_MAX, 2));
            entity.SetAnimationBool("ArrowVisible", !GetBowFired(entity));

            entity.SetModelDamagePercent();
        }

        private void PullBow(Entity entity)
        {
            bool bowFired = GetBowFired(entity);
            var bowPower = GetBowPower(entity);
            if (!bowFired)
            {
                bowPower += (int)(entity.GetAttackSpeed() * BOW_POWER_PULL_SPEED);
                if (bowPower >= BOW_POWER_MAX)
                {
                    bowPower = BOW_POWER_MAX;
                    SetBowFired(entity, true);

                    entity.ShootProjectile();
                }
            }
            else
            {
                bowPower -= (int)(entity.GetAttackSpeed() * BOW_POWER_RESTORE_SPEED);
                if (bowPower <= 0)
                {
                    bowPower = 0;
                    SetBowFired(entity, false);
                }
            }
            SetBowPower(entity, bowPower);
        }

        private void UnleaseBow(Entity enemy)
        {
            SetBowFired(enemy, false);
            var bowPower = GetBowPower(enemy);
            if (bowPower > 0)
            {
                bowPower -= (int)(enemy.GetAttackSpeed() * BOW_POWER_RESTORE_SPEED);
                bowPower = Mathf.Max(bowPower, 0);
            }
            SetBowPower(enemy, bowPower);
        }

        private bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }
        private Entity? FindTarget(Entity entity)
        {
            var collider = detector.Detect(entity);
            return collider?.Entity;
        }
        private bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);

            // 检查是否应被移除（例如被完全消灭），是则不刷怪  
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            // 生成随机骷髅  
            SpawnRandomEnemy(entity);
            entity.Remove();
        }

        /// <summary>  
        /// 在原位置生成随机骷髅  
        /// </summary>  
        private void SpawnRandomEnemy(Entity entity)
        {
            var rng = entity.RNG;

            // 使用白名单和权重随机  
            NamespaceID enemyID = GetRandomEnemyID(rng);

            // 在原位置生成骷髅  
            var spawnParam = entity.GetSpawnParams();
            spawnParam.SetProperty(EngineEntityProps.FACTION, entity.GetFaction());
            entity.Spawn(enemyID, entity.Position, spawnParam);
        }

        /// <summary>  
        /// 根据权重随机选择一个骷髅ID  
        /// </summary>  
        private NamespaceID GetRandomEnemyID(RandomGenerator rng)
        {
            var index = rng.WeightedRandom(SpawnWeights);
            return SpawnWhitelist[index];
        }

        // 允许生成的骷髅白名单  
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]
        {
            VanillaEnemyID.skeleton,
            VanillaEnemyID.skeletonHorse,
            VanillaEnemyID.skeletonWarrior,
            VanillaEnemyID.skeletonMage,
            VanillaEnemyID.SkeletonHead,
            VanillaEnemyID.KingSkeleton,
            VanillaEnemyID.MeleeSkeleton,
            VanillaEnemyID.RandomKingSkeleton,
        };

        // 每种骷髅的生成权重  
        private static int[] SpawnWeights = new int[]
        {
            1, 1, 1, 1, 1, 1, 1, 1,
        };

        public static int GetBowPower(Entity enemy) => enemy.GetBehaviourField<int>(ID, PROP_BOW_POWER);
        public static bool GetBowFired(Entity enemy) => enemy.GetBehaviourField<bool>(ID, PROP_BOW_FIRED);
        public static void SetBowPower(Entity enemy, int value) => enemy.SetBehaviourField(ID, PROP_BOW_POWER, value);
        public static void SetBowFired(Entity enemy, bool value) => enemy.SetBehaviourField(ID, PROP_BOW_FIRED, value);

        private Detector detector;
        private static readonly NamespaceID ID = VanillaEnemyID.RandomSkeleton;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_BOW_FIRED = new VanillaEntityPropertyMeta<bool>("bowFired");
        public static readonly VanillaEntityPropertyMeta<int> PROP_BOW_POWER = new VanillaEntityPropertyMeta<int>("bowPower");
        public const int STATE_WALK = LogicEnemyStates.WALK;
        public const int STATE_RANGED_ATTACK = LogicEnemyStates.RANGED_ATTACK;
        public const int BOW_POWER_PULL_SPEED = 100;
        public const int BOW_POWER_RESTORE_SPEED = 1000;
        public const int BOW_POWER_MAX = 10000;
    }
}
