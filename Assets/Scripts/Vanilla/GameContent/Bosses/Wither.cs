using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Bosses
{
    [EntityBehaviourDefinition(VanillaBossNames.wither)]
    public partial class Wither : BossBehaviour
    {
        public Wither(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_PROJECTILE_HIT, PreProjectileHitCallback);
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            stateMachine.Init(boss);
            stateMachine.StartState(boss, STATE_IDLE);

            boss.CollisionMaskHostile |=
                EntityCollisionHelper.MASK_PLANT |
                EntityCollisionHelper.MASK_ENEMY |
                EntityCollisionHelper.MASK_OBSTACLE |
                EntityCollisionHelper.MASK_BOSS;
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);

            if (entity.IsTimeInterval(CRY_INTERVAL))
            {
                entity.PlaySound(VanillaSoundID.witherCry);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);
            entity.SetAnimationBool("Armored", HasArmor(entity));
            entity.SetAnimationFloat("HeadOpen", GetHeadOpen(entity));

            RotateHeadsUpdate(entity);

            if (!entity.IsDead)
            {
                var regen = entity.Level.GetWitherRegeneration();
                entity.Heal(regen, entity);
            }
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            boss.PlaySound(VanillaSoundID.witherDeath);
            stateMachine.StartState(boss, STATE_DEATH);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var other = collision.Other;
            var self = collision.Entity;
            if (!other.IsHostile(self))
                return;
            var otherCollider = collision.OtherCollider;
            var crushDamage = 1000000;
            var substate = stateMachine.GetSubState(self);
            if (other.IsInvincible())
            {
                if (self.State == STATE_CHARGE && substate == ChargeState.SUBSTATE_DASH)
                {
                    Stun(self);
                }
            }
            else
            {
                var result = otherCollider.TakeDamage(crushDamage, new DamageEffectList(VanillaDamageEffects.DAMAGE_BODY_AFTER_ARMOR_BROKEN), self);
                if (result != null && result.HasAnyFatal())
                {
                    other.PlaySound(VanillaSoundID.smash);
                    if (self.State == STATE_EAT && (substate == EatState.SUBSTATE_DASH || substate == EatState.SUBSTATE_EATEN))
                    {
                        if (other.IsEntityOf(VanillaContraptionID.goldenApple))
                        {
                            Stun(self);
                            self.TakeDamage(GOLDEN_APPLE_DAMAGE, new DamageEffectList(VanillaDamageEffects.IGNORE_ARMOR), other);
                        }
                        else
                        {
                            self.HealEffects(EAT_HEALING, other);
                        }
                    }
                }
            }
            if (self.State == STATE_EAT && substate == EatState.SUBSTATE_DASH)
            {
                FinishEat(self);
            }
        }
        public override void PreTakeDamage(DamageInput damageInfo, CallbackResult result)
        {
            base.PreTakeDamage(damageInfo, result);
            if (damageInfo.Amount > 600)
            {
                damageInfo.SetAmount(600);
            }
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);

            var entity = result.Entity;
            //取消蓄能
            if (entity.State == STATE_CHARGE)
            {
                var substate = stateMachine.GetSubState(entity);
                if (substate == ChargeState.SUBSTATE_CHARGING)
                {
                    Stun(entity);
                    return;
                }
            }
            if (result.BodyResult != null)
            {
                var source = result.BodyResult.Source;
                var effects = result.BodyResult.Effects;
                var sourceEnt = source.GetEntity(entity.Level);
                if (sourceEnt != null && sourceEnt.IsEntityOf(VanillaEnemyID.bedserker) && effects.HasEffect(VanillaDamageEffects.EXPLOSION))
                {
                    Stun(entity);
                    return;
                }
            }
        }
        private void PreProjectileHitCallback(VanillaLevelCallbacks.PreProjectileHitParams param, CallbackResult result)
        {
            var hit = param.hit;
            var self = hit.Other;
            if (!self.IsEntityOf(VanillaBossID.wither))
                return;
            if (!HasArmor(self))
                return;
            result.SetFinalValue(false);
            var projectile = hit.Projectile;
            projectile.Remove();
        }
        #endregion 事件

        #region 字段
        public static int GetPhase(Entity entity) => entity.GetBehaviourField<int>(PROP_PHASE);
        public static void SetPhase(Entity entity, int value) => entity.SetBehaviourField(PROP_PHASE, value);
        public static float[] GetHeadAngles(Entity entity) => entity.GetBehaviourField<float[]>(PROP_HEAD_ANGLES);
        public static void SetHeadAngles(Entity entity, float[] value) => entity.SetBehaviourField(PROP_HEAD_ANGLES, value);
        public static float GetHeadOpen(Entity entity) => entity.GetBehaviourField<float>(PROP_HEAD_OPEN);
        public static void SetHeadOpen(Entity entity, float value) => entity.SetBehaviourField(PROP_HEAD_OPEN, value);
        public static EntityID[] GetHeadTargets(Entity entity) => entity.GetBehaviourField<EntityID[]>(PROP_HEAD_TARGETS);
        public static void SetHeadTargets(Entity entity, EntityID[] value) => entity.SetBehaviourField(PROP_HEAD_TARGETS, value);
        public static float[] GetSkullCharges(Entity entity) => entity.GetBehaviourField<float[]>(PROP_SKULL_CHARGES);
        public static void SetSkullCharges(Entity entity, float[] value) => entity.SetBehaviourField(PROP_SKULL_CHARGES, value);
        public static int GetTargetLane(Entity entity) => entity.GetBehaviourField<int>(PROP_TARGET_LANE);
        public static void SetTargetLane(Entity entity, int value) => entity.SetBehaviourField(PROP_TARGET_LANE, value);
        #endregion

        private void Stun(Entity entity)
        {
            if (entity.IsDead)
                return;
            entity.PlaySound(VanillaSoundID.witherDamage);
            entity.SetAnimationBool("Shaking", true);
            entity.TriggerAnimation("Interrupt");
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.StartState(entity, STATE_STUNNED);
        }
        private static void FinishEat(Entity entity)
        {
            if (entity.State != STATE_EAT)
                return;
            var subState = stateMachine.GetSubState(entity);
            if (subState != EatState.SUBSTATE_DASH)
                return;
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.SetSubState(entity, EatState.SUBSTATE_EATEN);
            var substateTimer = stateMachine.GetSubStateTimer(entity);
            substateTimer.ResetTime(20);
        }
        private void RotateHeadsUpdate(Entity entity)
        {
            EntityID[] headTargets = GetHeadTargets(entity);
            if (headTargets == null)
                return;
            var headAngles = GetHeadAngles(entity);
            if (headAngles == null)
            {
                headAngles = new float[HEAD_COUNT];
                SetHeadAngles(entity, headAngles);
            }
            for (var head = 0; head < headTargets.Length; head++)
            {
                var target = headTargets[head]?.GetEntity(entity.Level);
                float targetAngle = 0;
                if (target.ExistsAndAlive())
                {
                    var headPosition = GetHeadPosition(entity, head);
                    if (target.ExistsAndAlive())
                    {
                        Vector3 targetDirection = target.GetCenter() - headPosition;
                        Vector3 facingDirection = entity.GetFacingDirection();
                        var targetDir2D = new Vector2(targetDirection.x, targetDirection.z);
                        var facingDir2D = new Vector2(facingDirection.x, facingDirection.z);
                        targetAngle = Vector2.SignedAngle(targetDir2D, facingDir2D);
                    }
                }

                var angle = headAngles[head];
                if (angle > 180)
                {
                    angle = angle - 360;
                }
                if (targetAngle > angle)
                {
                    if (angle + HEAD_ROTATE_SPEED > targetAngle)
                    {
                        angle = targetAngle;
                    }
                    else
                    {
                        angle += HEAD_ROTATE_SPEED;
                    }
                }
                else
                {
                    if (angle - HEAD_ROTATE_SPEED < targetAngle)
                    {
                        angle = targetAngle;
                    }
                    else
                    {
                        angle -= HEAD_ROTATE_SPEED;
                    }
                }
                angle = (angle + 360) % 360;
                headAngles[head] = angle;

                switch (head)
                {
                    case HEAD_MAIN:
                        entity.SetAnimationFloat("MainHeadRotation", angle);
                        break;
                    case HEAD_RIGHT:
                        entity.SetAnimationFloat("RightHeadRotation", angle);
                        break;
                    case HEAD_LEFT:
                        entity.SetAnimationFloat("LeftHeadRotation", angle);
                        break;
                }
            }
        }
        private static IEnumerable<int> FindChargeLanes(Entity entity)
        {
            findChargeLaneBuffer.Clear();
            findChargeLaneDetector.DetectEntities(entity, findChargeLaneBuffer);
            if (findChargeLaneBuffer.Count <= 0)
                return null;
            var groups = findChargeLaneBuffer.GroupBy(e => e.GetLane());
            return groups.Select(g => g.Key);
        }
        private static bool CanCharge(Entity entity)
        {
            var lanes = FindChargeLanes(entity);
            return lanes != null && lanes.Count() > 0;
        }
        private static int FindChargeLane(Entity entity)
        {
            var lanes = FindChargeLanes(entity);
            if (lanes == null || lanes.Count() <= 0)
                return -1;
            return lanes.Random(entity.RNG);
        }
        private static IEnumerable<int> FindEatLanes(Entity entity)
        {
            findEatLaneBuffer.Clear();
            findEatLaneDetector.DetectEntities(entity, findEatLaneBuffer);
            if (findEatLaneBuffer.Count <= 0)
                return null;
            var groups = findEatLaneBuffer.GroupBy(e => e.GetLane());
            return groups.Select(g => g.Key);
        }
        private static bool CanEat(Entity entity)
        {
            var lanes = FindEatLanes(entity);
            return lanes != null && lanes.Count() > 0;
        }
        private static int FindEatLane(Entity entity)
        {
            var lanes = FindEatLanes(entity);
            if (lanes == null || lanes.Count() <= 0)
                return -1;
            return lanes.Random(entity.RNG);
        }
        public static bool HasArmor(Entity entity)
        {
            return GetPhase(entity) == PHASE_2 && !entity.IsDead;
        }
        public static Vector3 GetHeadPosition(Entity entity, int head)
        {
            return entity.Position + headPositionOffsets[head];
        }
        public static void Appear(Entity entity)
        {
            stateMachine.StartState(entity, STATE_APPEAR);
            entity.PlaySound(VanillaSoundID.witherSpawn);
            entity.PlaySound(VanillaSoundID.witherDeath);
        }

        #region 常量
        private static readonly VanillaEntityPropertyMeta<float[]> PROP_HEAD_ANGLES = new VanillaEntityPropertyMeta<float[]>("HeadAngles");
        private static readonly VanillaEntityPropertyMeta<float> PROP_HEAD_OPEN = new VanillaEntityPropertyMeta<float>("HeadOpen");
        private static readonly VanillaEntityPropertyMeta<EntityID[]> PROP_HEAD_TARGETS = new VanillaEntityPropertyMeta<EntityID[]>("HeadTargets");
        private static readonly VanillaEntityPropertyMeta<float[]> PROP_SKULL_CHARGES = new VanillaEntityPropertyMeta<float[]>("SkullCharges");
        private static readonly VanillaEntityPropertyMeta<int> PROP_PHASE = new VanillaEntityPropertyMeta<int>("Phase");
        private static readonly VanillaEntityPropertyMeta<int> PROP_TARGET_LANE = new VanillaEntityPropertyMeta<int>("TargetLane");

        private static readonly Vector3[] headPositionOffsets = new Vector3[]
        {
            new Vector3(0, 128, 0),
            new Vector3(0, 104, -16),
            new Vector3(0, 104, 16),
        };

        private const int STATE_IDLE = VanillaEntityStates.WITHER_IDLE;
        private const int STATE_APPEAR = VanillaEntityStates.WITHER_APPEAR;
        private const int STATE_CHARGE = VanillaEntityStates.WITHER_CHARGE;
        private const int STATE_EAT = VanillaEntityStates.WITHER_EAT;
        private const int STATE_SWITCH = VanillaEntityStates.WITHER_SWITCH;
        private const int STATE_SUMMON = VanillaEntityStates.WITHER_SUMMON;
        private const int STATE_STUNNED = VanillaEntityStates.WITHER_STUNNED;
        private const int STATE_DEATH = VanillaEntityStates.WITHER_DEATH;

        public const int PHASE_1 = 0;
        public const int PHASE_2 = 1;

        public const int HEAD_MAIN = 0;
        public const int HEAD_RIGHT = 1;
        public const int HEAD_LEFT = 2;

        public const int CRY_INTERVAL = 100;
        public const int HEAD_COUNT = 3;
        public const float HEAD_ROTATE_SPEED = 10;
        public const float FLY_HEIGHT = 80;
        public const float EAT_HEALING = 300;
        public const float GOLDEN_APPLE_DAMAGE = 600;
        #endregion 常量

        private static WitherStateMachine stateMachine = new WitherStateMachine();
        private static Detector findChargeLaneDetector = new WitherDetector(WitherDetector.MODE_CHARGE);
        private static Detector findEatLaneDetector = new WitherDetector(WitherDetector.MODE_EAT);
        private static List<Entity> findChargeLaneBuffer = new List<Entity>();
        private static List<Entity> findEatLaneBuffer = new List<Entity>();
    }
}
