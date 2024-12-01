using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Recharges;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [Definition(VanillaEnemyNames.skeleton)]
    public class Skeleton : StateEnemy
    {
        public Skeleton(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
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

            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
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
