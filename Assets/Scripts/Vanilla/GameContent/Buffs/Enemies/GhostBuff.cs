using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.ghost)]
    public class GhostBuff : BuffDefinition
    {
        public GhostBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULTIPLIER));
            AddModifier(new BooleanModifier(VanillaEntityProps.ETHEREAL, PROP_ETHEREAL));
            AddModifier(new FloatModifier(VanillaEntityProps.SHADOW_ALPHA, NumberOperator.Multiply, PROP_SHADOW_ALPHA));
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TINT_MULTIPLIER, new Color(1, 1, 1, GetMinAlpha(buff)));
            buff.SetProperty(PROP_ETHEREAL, true);
            buff.SetProperty(PROP_SHADOW_ALPHA, SHADOW_ALPHA_MIN);
            UpdateIllumination(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateIllumination(buff);
        }
        private void PreEntityTakeDamageCallback(DamageInput damageInfo)
        {
            var entity = damageInfo.Entity;
            if (entity == null)
                return;
            var buffs = entity.GetBuffs<GhostBuff>();
            if (buffs.Length <= 0)
                return;
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.WHACK))
                return;
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.FIRE))
            {
                foreach (var buff in buffs)
                {
                    buff.SetProperty(PROP_ETHEREAL, false);
                    SetEverIlluminated(buff, true);
                }
            }
            if (buffs.Any(b => b.GetProperty<bool>(PROP_ETHEREAL)))
            {
                damageInfo.Multiply(0.1f);
            }
        }
        private void UpdateIllumination(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            bool illuminated = entity.IsIlluminated() || entity.IsAIFrozen();
            SetIlluminated(buff, illuminated);
        }
        public static void SetIlluminated(Buff buff, bool illuminated)
        {
            if (illuminated)
            {
                SetEverIlluminated(buff, true);
            }
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            float tintSpeed = illuminated ? TINT_SPEED : -TINT_SPEED;
            float shadowSpeed = illuminated ? SHADOW_ALPHA_SPEED : -SHADOW_ALPHA_SPEED;
            bool ethereal = illuminated ? false : true;

            var tint = buff.GetProperty<Color>(PROP_TINT_MULTIPLIER);
            tint.a = Mathf.Clamp(tint.a + tintSpeed, GetMinAlpha(buff), TINT_ALPHA_MAX);

            var shadowAlpha = buff.GetProperty<float>(PROP_SHADOW_ALPHA);
            shadowAlpha = Mathf.Clamp(shadowAlpha + shadowSpeed, SHADOW_ALPHA_MIN, SHADOW_ALPHA_MAX);

            buff.SetProperty(PROP_TINT_MULTIPLIER, tint);
            buff.SetProperty(PROP_SHADOW_ALPHA, shadowAlpha);
            buff.SetProperty(PROP_ETHEREAL, ethereal);
        }
        public static void Illuminate(Buff buff)
        {
            buff.SetProperty(PROP_TINT_MULTIPLIER, Color.white);
            buff.SetProperty(PROP_SHADOW_ALPHA, SHADOW_ALPHA_MAX);
            buff.SetProperty(PROP_ETHEREAL, false);
        }
        private static float GetMinAlpha(Buff buff)
        {
            if (buff.Level.StageDefinition is WhackAGhostStage)
            {
                return 0;
            }
            return TINT_ALPHA_MIN;
        }
        public static void SetEverIlluminated(Buff buff, bool value)
        {
            buff.SetProperty(PROP_EVER_ILLUMINATED, value);
        }
        public static bool IsEverIlluminated(Buff buff)
        {
            return buff.GetProperty<bool>(PROP_EVER_ILLUMINATED);
        }
        public static bool IsEverIlluminated(Entity entity)
        {
            return entity.GetBuffs<GhostBuff>().Any(b => GhostBuff.IsEverIlluminated(b));
        }
        public const string PROP_EVER_ILLUMINATED = "EverIlluminated";
        public const string PROP_TINT_MULTIPLIER = "TintMultiplier";
        public const string PROP_SHADOW_ALPHA = "ShadowAlpha";
        public const string PROP_ETHEREAL = "Ethereal";
        public const float TINT_ALPHA_MIN = 0.5f;
        public const float TINT_ALPHA_MAX = 1;
        public const float TINT_SPEED = 0.02f;
        public const float SHADOW_ALPHA_MIN = 0;
        public const float SHADOW_ALPHA_MAX = 1;
        public const float SHADOW_ALPHA_SPEED = 0.04f;
    }
}
