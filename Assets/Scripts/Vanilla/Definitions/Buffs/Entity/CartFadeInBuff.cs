using MVZ2.GameContent;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.Vanilla.Buffs
{
    [Definition(VanillaBuffNames.cartFadeIn)]
    public class CartFadeInBuff : BuffDefinition
    {
        public CartFadeInBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_COLOR_MULTIPLIER));
            AddModifier(ColorModifier.Multiply(BuiltinEntityProps.LIGHT_COLOR, PROP_COLOR_MULTIPLIER));
            AddModifier(new FloatModifier(BuiltinEntityProps.SHADOW_ALPHA, NumberOperator.Multiply, PROP_ALPHA_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateColor(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateColor(buff);
        }
        private void UpdateColor(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var alpha = Mathf.Lerp(0, 1, (entity.Position.x - BuiltinLevel.CART_START_X) / (BuiltinLevel.CART_TARGET_X - BuiltinLevel.CART_START_X));
            buff.SetProperty(PROP_COLOR_MULTIPLIER, new Color(1, 1, 1, alpha));
            buff.SetProperty(PROP_ALPHA_MULTIPLIER, alpha);
        }
        public const string PROP_COLOR_MULTIPLIER = "ColorMultiplier";
        public const string PROP_ALPHA_MULTIPLIER = "AlphaMultiplier";
    }
}
