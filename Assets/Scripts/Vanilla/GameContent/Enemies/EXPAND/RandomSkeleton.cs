using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
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
    public class RandomSkeleton : StateEnemy
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
            base.UpdateAI(enemy);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationFloat("BowBlend", 1 - Mathf.Pow(1 - GetBowPower(entity) / (float)BOW_POWER_MAX, 2));
            entity.SetAnimationBool("ArrowVisible", !GetBowFired(entity));

            entity.SetModelDamagePercent();
        }
        public static int GetBowPower(Entity enemy) => enemy.GetBehaviourField<int>(ID, PROP_BOW_POWER);
        public static bool GetBowFired(Entity enemy) => enemy.GetBehaviourField<bool>(ID, PROP_BOW_FIRED);
        public static void SetBowPower(Entity enemy, int value) => enemy.SetBehaviourField(ID, PROP_BOW_POWER, value);
        public static void SetBowFired(Entity enemy, bool value) => enemy.SetBehaviourField(ID, PROP_BOW_FIRED, value);
        protected override void UpdateStateWalk(Entity enemy)
        {
            base.UpdateStateWalk(enemy);
            SetBowFired(enemy, false);
            var bowPower = GetBowPower(enemy);
            if (bowPower > 0)
            {
                bowPower -= (int)(enemy.GetAttackSpeed() * BOW_POWER_RESTORE_SPEED);
                bowPower = Mathf.Max(bowPower, 0);
            }
            SetBowPower(enemy, bowPower);
        }
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);
            RangedAttack(enemy, enemy.Target);
        }
        protected virtual bool CanShoot(Entity enemy)
        {
            return enemy.Position.x <= enemy.Level.GetEntityColumnX(enemy.Level.GetMaxColumnCount() - 1);
        }
        protected virtual Entity FindTarget(Entity entity)
        {
            var collider = detector.Detect(entity);
            return collider?.Entity;
        }
        protected virtual bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.ValidateTarget(entity, target);
        }
        protected virtual void RangedAttack(Entity entity, Entity target)
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

        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);

            // ����Ƿ�Ӧ���Ƴ�ʵ��������ɹ���  
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;

            // ���������ʬ  
            SpawnRandomEnemy(entity);
            entity.Remove();
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
            VanillaEnemyID.skeleton,
            VanillaEnemyID.skeletonHorse,
            VanillaEnemyID.skeletonWarrior,
            VanillaEnemyID.skeletonMage,
            VanillaEnemyID.SkeletonHead,
            VanillaEnemyID.KingSkeleton,
            VanillaEnemyID.MeleeSkeleton,
            VanillaEnemyID.RandomKingSkeleton,

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
            1,

        };

        private Detector detector;
        private static readonly NamespaceID ID = VanillaEnemyID.RandomSkeleton;
        public static readonly VanillaEntityPropertyMeta<bool> PROP_BOW_FIRED = new VanillaEntityPropertyMeta<bool>("bowFired");
        public static readonly VanillaEntityPropertyMeta<int> PROP_BOW_POWER = new VanillaEntityPropertyMeta<int>("bowPower");
        public const int BOW_POWER_PULL_SPEED = 100;
        public const int BOW_POWER_RESTORE_SPEED = 1000;
        public const int BOW_POWER_MAX = 10000;
    }
}
