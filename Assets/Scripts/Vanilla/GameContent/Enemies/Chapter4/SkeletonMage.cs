using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.skeletonMage)]
    public class SkeletonMage : StateEnemy
    {
        public SkeletonMage(string nsp, string name) : base(nsp, name)
        {
            detector = new DispenserDetector()
            {
                ignoreHighEnemy = true,
                ignoreLowEnemy = true
            };
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetStateTimer(entity, new FrameTimer(ATTACK_CAST_TIME));
            entity.SetVariant(mageVariants.Random(entity.RNG));
            entity.SetAnimationInt("Class", entity.GetVariant());
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
            entity.SetAnimationInt("Class", entity.GetVariant());
        }
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);
            if (state == VanillaEntityStates.ATTACK)
            {
                if (GetAttackState(enemy) == ATTACK_STATE_CAST)
                {
                    state = VanillaEntityStates.ENEMY_CAST;
                }
            }
            return state;
        }
        protected override void UpdateStateWalk(Entity enemy)
        {
            base.UpdateStateWalk(enemy);
            UpdateTarget(enemy);
            var timer = GetStateTimer(enemy);
            if (timer != null)
            {
                timer.ResetTime(ATTACK_CAST_TIME);
            }
        }
        protected override void UpdateStateCast(Entity enemy)
        {
            base.UpdateStateCast(enemy);
            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;
            timer.Run(enemy.GetAttackSpeed());
            if (timer.Expired)
            {
                SetAttackState(enemy, ATTACK_STATE_FIRE);
                timer.ResetTime(ATTACK_FIRE_TIME);
            }
        }
        protected override void UpdateStateAttack(Entity enemy)
        {
            base.UpdateStateAttack(enemy);
            var timer = GetStateTimer(enemy);
            if (timer == null)
                return;
            timer.Run(enemy.GetAttackSpeed());
            if (timer.Expired)
            {
                var attackState = GetAttackState(enemy);
                if (attackState == ATTACK_STATE_FIRE)
                {
                    SetAttackState(enemy, ATTACK_STATE_RESTORE);
                    timer.ResetTime(ATTACK_RESTORE_TIME);
                    Shoot(enemy);
                }
                else if (attackState == ATTACK_STATE_RESTORE)
                {
                    UpdateTarget(enemy);
                    SetAttackState(enemy, ATTACK_STATE_CAST);
                    timer.ResetTime(ATTACK_CAST_TIME);
                }
            }
        }
        private void UpdateTarget(Entity enemy)
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
        }
        private void Shoot(Entity enemy)
        {
            var enemyClass = enemy.GetVariant();
            switch (enemyClass)
            {
                case VARIANT_FROST:
                    {
                        var param = enemy.GetShootParams();
                        param.damage = enemy.GetDamage() * 0.2f;
                        param.projectileID = VanillaProjectileID.iceBolt;
                        param.soundID = VanillaSoundID.snow;
                        enemy.ShootProjectile(param);
                    }
                    break;
                case VARIANT_LIGHTNING:
                    {
                        var param = enemy.GetShootParams();
                        param.damage = enemy.GetDamage() * 0.2f;
                        param.projectileID = VanillaProjectileID.chargedBolt;
                        param.soundID = null;
                        param.velocity *= 0.4f;
                        for (int i = 0; i < 3; i++)
                        {
                            enemy.ShootProjectile(param);
                        }
                    }
                    break;
                default:
                    {
                        var param = enemy.GetShootParams();
                        param.damage = enemy.GetDamage() * 0.8f;
                        param.projectileID = VanillaProjectileID.fireball;
                        param.soundID = VanillaSoundID.fire;
                        enemy.ShootProjectile(param);
                    }
                    break;

            }
        }
        public static void SetStateTimer(Entity enemy, FrameTimer value) => enemy.SetBehaviourField(PROP_STATE_TIMER, value);
        public static FrameTimer GetStateTimer(Entity enemy) => enemy.GetBehaviourField<FrameTimer>(PROP_STATE_TIMER);
        public static void SetAttackState(Entity enemy, int value) => enemy.SetBehaviourField(PROP_ATTACK_STATE, value);
        public static int GetAttackState(Entity enemy) => enemy.GetBehaviourField<int>(PROP_ATTACK_STATE);
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
        private Detector detector;
        public const int ATTACK_STATE_CAST = 0;
        public const int ATTACK_STATE_FIRE = 1;
        public const int ATTACK_STATE_RESTORE = 2;

        public const int ATTACK_CAST_TIME = 5;
        public const int ATTACK_FIRE_TIME = 5;
        public const int ATTACK_RESTORE_TIME = 20;

        public const int VARIANT_FIRE = 0;
        public const int VARIANT_FROST = 1;
        public const int VARIANT_LIGHTNING = 2;
        public static readonly VanillaEntityPropertyMeta<int> PROP_ATTACK_STATE = new VanillaEntityPropertyMeta<int>("attackState");
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("attackTimer");
        public static int[] mageVariants = new int[]
        {
            SkeletonMage.VARIANT_FIRE,
            SkeletonMage.VARIANT_FROST,
            SkeletonMage.VARIANT_LIGHTNING
        };
    }
}
