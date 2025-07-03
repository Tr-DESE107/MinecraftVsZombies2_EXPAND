using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2Logic.Level;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.redstone)]
    public class Redstone : PickupBehaviour
    {
        public Redstone(string nsp, string name) : base(nsp, name)
        {
            SetProperty(PROP_TINT_MULT, Color.white);
            AddModifier(ColorModifier.Multiply(EngineEntityProps.TINT, PROP_TINT_MULT));
            AddModifier(new FloatModifier(VanillaEntityProps.SHADOW_ALPHA, NumberOperator.Multiply, PROP_SHADOW_ALPHA));
        }
        public override void Update(Entity pickup)
        {
            base.Update(pickup);
            float alpha = 1;
            float shadowAlpha = 1;
            if (!pickup.IsCollected() && !pickup.IsImportantPickup() && pickup.Timeout < 15)
            {
                alpha = pickup.Timeout / 15f;
                shadowAlpha = alpha;
            }
            var color = GetTintMultiplier(pickup);
            color.a = alpha;
            SetTintMultiplier(pickup, color);
            SetShadowAlpha(pickup, shadowAlpha);
        }
        public static Color GetTintMultiplier(Entity entity) => entity.GetBehaviourField<Color>(PROP_TINT_MULT);
        public static void SetTintMultiplier(Entity entity, Color value) => entity.SetBehaviourField(PROP_TINT_MULT, value);
        public static float GetShadowAlpha(Entity entity) => entity.GetBehaviourField<float>(PROP_SHADOW_ALPHA);
        public static void SetShadowAlpha(Entity entity, float value) => entity.SetBehaviourField(PROP_SHADOW_ALPHA, value);
        private static readonly VanillaEntityPropertyMeta<Color> PROP_TINT_MULT = new VanillaEntityPropertyMeta<Color>("tint_mult");
        private static readonly VanillaEntityPropertyMeta<float> PROP_SHADOW_ALPHA = new VanillaEntityPropertyMeta<float>("shadow_alpha");
    }
}