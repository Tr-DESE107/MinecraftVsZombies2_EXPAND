using System;
using MVZ2.GameContent;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(VanillaBuffNames.ghost)]
    public class GhostBuff : BuffDefinition
    {
        public GhostBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.TINT, BlendOperator.DstColor, BlendOperator.Zero, PROP_TINT_MULTIPLIER));
            AddModifier(new BooleanModifier(VanillaEntityProps.ETHEREAL, PROP_ETHEREAL));
            AddTrigger(LevelCallbacks.PreEntityTakeDamage, PreEntityTakeDamageCallback);
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TINT_MULTIPLIER, new Color(1, 1, 1, TINT_ALPHA_MIN));
            buff.SetProperty(PROP_ETHEREAL, true);
            UpdateIllumination(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateIllumination(buff);
        }
        private void PreEntityTakeDamageCallback(Buff buff, DamageInfo damageInfo)
        {
            if (damageInfo.Entity != buff.Target)
                return;
            if (buff.GetProperty<bool>(PROP_ETHEREAL))
            {
                damageInfo.Multiply(0.1f);
            }
        }
        private void UpdateIllumination(Buff buff)
        {
            var entity = buff.Target.GetEntity();
            if (entity == null)
                return;
            bool illuminated = entity.IsIlluminated() || entity.IsAIFrozen();
            float alphaSpeed = illuminated ? TINT_SPEED : -TINT_SPEED;
            bool ethereal = illuminated ? false : true;
            var tint = buff.GetProperty<Color>(PROP_TINT_MULTIPLIER);
            tint.a = Mathf.Clamp(tint.a + alphaSpeed, TINT_ALPHA_MIN, TINT_ALPHA_MAX);
            buff.SetProperty(PROP_TINT_MULTIPLIER, tint);
            buff.SetProperty(PROP_ETHEREAL, ethereal);
        }
        public const string PROP_TINT_MULTIPLIER = "TintMultiplier";
        public const string PROP_ETHEREAL = "Ethereal";
        public const float TINT_ALPHA_MIN = 0.5f;
        public const float TINT_ALPHA_MAX = 1;
        public const float TINT_SPEED = 0.02f;
    }
}
