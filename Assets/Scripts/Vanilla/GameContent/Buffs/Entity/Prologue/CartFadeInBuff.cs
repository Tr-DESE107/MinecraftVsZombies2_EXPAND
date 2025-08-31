using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Carts
{
    [BuffDefinition(VanillaBuffNames.Cart.cartFadeIn)]
    public class CartFadeInBuff : BuffDefinition
    {
        public CartFadeInBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_COLOR_MULTIPLIER));
            AddModifier(ColorModifier.Multiply(VanillaEntityProps.LIGHT_COLOR, PROP_COLOR_MULTIPLIER));
            AddModifier(new FloatModifier(VanillaEntityProps.SHADOW_ALPHA, NumberOperator.Multiply, PROP_ALPHA_MULTIPLIER));
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
            var alpha = Mathf.Lerp(0, 1, (entity.Position.x - VanillaLevelExt.CART_START_X) / (VanillaLevelExt.CART_TARGET_X - VanillaLevelExt.CART_START_X));
            buff.SetProperty(PROP_COLOR_MULTIPLIER, new Color(1, 1, 1, alpha));
            buff.SetProperty(PROP_ALPHA_MULTIPLIER, alpha);
        }
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR_MULTIPLIER = new VanillaBuffPropertyMeta<Color>("ColorMultiplier");
        public static readonly VanillaBuffPropertyMeta<float> PROP_ALPHA_MULTIPLIER = new VanillaBuffPropertyMeta<float>("AlphaMultiplier");
    }
}
