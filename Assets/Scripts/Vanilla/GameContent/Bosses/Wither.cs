using MVZ2.GameContent.Detections;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
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
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            stateMachine.UpdateAI(entity);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            stateMachine.UpdateLogic(entity);
            entity.SetAnimationBool("Armored", HasArmor(entity));
            entity.SetAnimationFloat("HeadOpen", GetHeadOpen(entity));

            var angles = GetHeadAngles(entity);
            float mainAngle = 0;
            float rightAngle = 0;
            float leftAngle = 0;
            if (angles != null)
            {
                if (angles.Length > 0)
                {
                    mainAngle = angles[0];
                }
                if (angles.Length > 1)
                {
                    rightAngle = angles[1];
                }
                if (angles.Length > 2)
                {
                    leftAngle = angles[2];
                }
            }
            entity.SetAnimationFloat("MainHeadRotation", mainAngle);
            entity.SetAnimationFloat("RightHeadRotation", rightAngle);
            entity.SetAnimationFloat("LeftHeadRotation", leftAngle);
        }
        public override void PostDeath(Entity boss, DeathInfo damageInfo)
        {
            base.PostDeath(boss, damageInfo);
            stateMachine.StartState(boss, STATE_DEATH);
        }
        public override void PreTakeDamage(DamageInput damageInfo)
        {
            base.PreTakeDamage(damageInfo);
            if (damageInfo.Amount > 600)
            {
                damageInfo.SetAmount(600);
            }
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
        #endregion

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
        public const int HEAD_COUNT = 3;
        public const float HEAD_ROTATE_SPEED = 10;
        public const float FLY_HEIGHT = 80;
        #endregion 常量

        private static WitherStateMachine stateMachine = new WitherStateMachine();
    }
}
