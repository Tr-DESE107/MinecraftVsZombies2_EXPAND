using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace MVZ2.GameContent.Enemies
{
    [Definition(EnemyNames.skeleton)]
    [SpawnDefinition(2)]
    [EntitySeedDefinition(50, VanillaMod.spaceName, RechargeNames.none)]
    public class Skeleton : StateEnemy
    {
        public Skeleton(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EntityProperties.SIZE, new Vector3(32, 86, 32));
            SetProperty(EntityProperties.SHELL, ShellID.bone);
            SetProperty(VanillaEntityProps.RANGE, -1f);
            SetProperty(VanillaEntityProps.SHOT_OFFSET, new Vector3(10, 40, 0));
            SetProperty(VanillaEntityProps.SHOT_VELOCITY, new Vector3(15, 0, 0));
            SetProperty(VanillaEntityProps.PROJECTILE_ID, ProjectileID.arrow);
            SetProperty(BuiltinEnemyProps.CRY_SOUND, SoundID.skeletonCry);
            SetProperty(BuiltinEntityProps.DEATH_SOUND, SoundID.skeletonDeath);
            detector = new DispenserDetector()
            {
                ignoreLowEnemy = true
            };
        }
        public override void Update(Entity enemy)
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
            base.Update(enemy);
            enemy.SetAnimationFloat("BowBlend", 1 - Mathf.Pow(1 - GetBowPower(enemy) / (float)BOW_POWER_MAX, 2));
            enemy.SetAnimationBool("ArrowVisible", !GetBowFired(enemy));

            int healthState;
            float maxHP = enemy.GetMaxHealth();
            if (enemy.Health > maxHP * 0.5f)
                healthState = 1;
            else
                healthState = 0;
            enemy.SetAnimationInt("HealthState", healthState);
        }
        public static int GetBowPower(Entity enemy)
        {
            return enemy.GetProperty<int>(PROP_BOW_POWER);
        }
        public static bool GetBowFired(Entity enemy)
        {
            return enemy.GetProperty<bool>(PROP_BOW_FIRED);
        }
        public static void SetBowPower(Entity enemy, int value)
        {
            enemy.SetProperty(PROP_BOW_POWER, value);
        }
        public static void SetBowFired(Entity enemy, bool value)
        {
            enemy.SetProperty(PROP_BOW_FIRED, value);
        }
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
            return detector.Detect(entity);
        }
        protected virtual bool ValidateTarget(Entity entity, Entity target)
        {
            return detector.Validate(entity, target);
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
        private Detector detector;
        public const string PROP_BOW_FIRED = "bowFired";
        public const string PROP_BOW_POWER = "bowPower";
        public const int BOW_POWER_PULL_SPEED = 100;
        public const int BOW_POWER_RESTORE_SPEED = 1000;
        public const int BOW_POWER_MAX = 10000;
    }
}
