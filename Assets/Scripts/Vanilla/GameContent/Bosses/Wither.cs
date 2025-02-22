using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
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
        }

        #region 回调
        public override void Init(Entity boss)
        {
            base.Init(boss);
            stateMachine.Init(boss);
            stateMachine.StartState(boss, STATE_IDLE);

            boss.CollisionMaskHostile |= EntityCollisionHelper.MASK_PLANT;
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);
            entity.SetRelativeY(Mathf.Max(0, entity.GetRelativeY() - 1));
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
                entity.Health = Mathf.Min(entity.GetMaxHealth(), entity.Health + REGENERATION_SPEED);
            }
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            stateMachine.StartState(boss, STATE_DEATH);
        }
        public override void PostCollision(EntityCollision collision, int state)
        {
            base.PostCollision(collision, state);
            var other = collision.Other;
            if (other.Type != EntityTypes.PLANT)
                return;
            var self = collision.Entity;
            if (!other.IsHostile(self))
                return;
            var otherCollider = collision.OtherCollider;
            var crushDamage = 1000000;
            if (other.IsInvincible())
            {
                if (self.State == STATE_CHARGE)
                {
                    var substate = stateMachine.GetSubState(self);
                    if (substate == ChargeState.SUBSTATE_DASH)
                    {
                        InterruptDash(self);
                    }
                }
            }
            else
            {
                var result = otherCollider.TakeDamage(crushDamage, new DamageEffectList(), self);
                if (result != null && result.HasAnyFatal())
                {
                    other.PlaySound(VanillaSoundID.smash);
                }
            }
        }
        public override void PreTakeDamage(DamageInput damageInfo)
        {
            base.PreTakeDamage(damageInfo);
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
            if (entity.State != STATE_CHARGE)
                return;
            var substate = stateMachine.GetSubState(entity);
            if (substate != ChargeState.SUBSTATE_CHARGING)
                return;
            InterruptDash(entity);
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
        public static int GetChargeTargetLane(Entity entity) => entity.GetBehaviourField<int>(PROP_CHARGE_TARGET_LANE);
        public static void SetChargeTargetLane(Entity entity, int value) => entity.SetBehaviourField(PROP_CHARGE_TARGET_LANE, value);
        #endregion

        private void InterruptDash(Entity entity)
        {
            entity.PlaySound(VanillaSoundID.witherDamage);
            entity.SetAnimationBool("Shaking", true);
            var vel = entity.Velocity;
            vel.x = 0;
            entity.Velocity = vel;
            stateMachine.SetSubState(entity, ChargeState.SUBSTATE_INTERRUPTED);
            var substateTimer = stateMachine.GetSubStateTimer(entity);
            substateTimer.ResetTime(30);
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
        public static bool HasArmor(Entity entity)
        {
            return GetPhase(entity) == PHASE_2;
        }
        public static Vector3 GetHeadPosition(Entity entity, int head)
        {
            return entity.Position + headPositionOffsets[head];
        }

        #region 常量
        private static readonly VanillaEntityPropertyMeta PROP_HEAD_ANGLES = new VanillaEntityPropertyMeta("HeadAngles");
        private static readonly VanillaEntityPropertyMeta PROP_HEAD_OPEN = new VanillaEntityPropertyMeta("HeadOpen");
        private static readonly VanillaEntityPropertyMeta PROP_HEAD_TARGETS = new VanillaEntityPropertyMeta("HeadTargets");
        private static readonly VanillaEntityPropertyMeta PROP_SKULL_CHARGES = new VanillaEntityPropertyMeta("SkullCharges");
        private static readonly VanillaEntityPropertyMeta PROP_PHASE = new VanillaEntityPropertyMeta("Phase");
        private static readonly VanillaEntityPropertyMeta PROP_CHARGE_TARGET_LANE = new VanillaEntityPropertyMeta("ChargeTargetLane");

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
        private const int STATE_SUMMON = VanillaEntityStates.WITHER_SUMMON;
        private const int STATE_DEATH = VanillaEntityStates.WITHER_DEATH;

        public const int PHASE_1 = 0;
        public const int PHASE_2 = 1;

        public const int HEAD_MAIN = 0;
        public const int HEAD_RIGHT = 1;
        public const int HEAD_LEFT = 2;

        public const int HEAD_COUNT = 3;
        public const float HEAD_ROTATE_SPEED = 10;
        public const float FLY_HEIGHT = 80;
        public const float REGENERATION_SPEED = 1 / 3f;
        #endregion 常量

        private static WitherStateMachine stateMachine = new WitherStateMachine();
        private static Detector findChargeLaneDetector = new WitherDetector();
        private static List<Entity> findChargeLaneBuffer = new List<Entity>();
    }
}
