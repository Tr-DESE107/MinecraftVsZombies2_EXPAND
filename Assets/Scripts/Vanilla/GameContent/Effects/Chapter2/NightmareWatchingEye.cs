using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Effects
{
    [EntityBehaviourDefinition(VanillaEffectNames.nightmareWatchingEye)]
    public class NightmareWatchingEye : EffectBehaviour
    {
        #region 公有方法
        public NightmareWatchingEye(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetEyeMoveCooldown(entity, 90);
        }
        public override void Update(Entity entity)
        {
            base.Update(entity);
            var cooldown = GetEyeMoveCooldown(entity);
            cooldown--;
            if (cooldown <= 0)
            {
                var rng = entity.RNG;
                cooldown = rng.Next(60, 150);
                var radius = rng.NextFloat() * 1;
                var angle = rng.NextFloat() * 360;
                var target = Vector2.right.RotateClockwise(angle) * radius;
                SetEyeTarget(entity, target);
            }
            SetEyeMoveCooldown(entity, cooldown);

            var eyeTarget = GetEyeTarget(entity);
            var eyeDirection = GetEyeDirection(entity);
            eyeDirection = eyeDirection * 0.5f + eyeTarget * 0.5f;
            SetEyeDirection(entity, eyeDirection);

            entity.SetAnimationBool("Open", entity.Timeout < 0);
            entity.SetModelProperty("EyeDirection", eyeDirection);
        }
        #endregion

        public static int GetEyeMoveCooldown(Entity entity)
        {
            return entity.GetBehaviourField<int>(ID, PROP_EYE_MOVE_COOLDOWN);
        }
        public static void SetEyeMoveCooldown(Entity entity, int value)
        {
            entity.SetBehaviourField(ID, PROP_EYE_MOVE_COOLDOWN, value);
        }
        public static Vector2 GetEyeDirection(Entity entity)
        {
            return entity.GetBehaviourField<Vector2>(ID, PROP_EYE_DIRECTION);
        }
        public static void SetEyeDirection(Entity entity, Vector2 value)
        {
            entity.SetBehaviourField(ID, PROP_EYE_DIRECTION, value);
        }
        public static Vector2 GetEyeTarget(Entity entity)
        {
            return entity.GetBehaviourField<Vector2>(ID, PROP_EYE_TARGET);
        }
        public static void SetEyeTarget(Entity entity, Vector2 value)
        {
            entity.SetBehaviourField(ID, PROP_EYE_TARGET, value);
        }
        public static readonly NamespaceID ID = VanillaEffectID.nightmareWatchingEye;
        public static readonly VanillaEntityPropertyMeta PROP_EYE_DIRECTION = new VanillaEntityPropertyMeta("EyeDirection");
        public static readonly VanillaEntityPropertyMeta PROP_EYE_TARGET = new VanillaEntityPropertyMeta("EyeTarget");
        public static readonly VanillaEntityPropertyMeta PROP_EYE_MOVE_COOLDOWN = new VanillaEntityPropertyMeta("EyeMoveCooldown");
    }
}