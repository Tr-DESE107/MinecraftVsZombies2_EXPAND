using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Projectiles
{
    [BuffDefinition(VanillaBuffNames.ghastFireCharge)]
    public class GhastFireChargeBuff : BuffDefinition
    {
        public GhastFireChargeBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.DAMAGE, NumberOperator.Multiply, PROP_DAMAGE_MULTIPLIER));
            AddModifier(new FloatModifier(VanillaEntityProps.RANGE, NumberOperator.Multiply, PROP_RANGE_MULTIPLIER));
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, PROP_SCALE_MULTIPLIER));
            AddModifier(new Vector3Modifier(EngineEntityProps.DISPLAY_SCALE, NumberOperator.Multiply, PROP_SCALE_MULTIPLIER));
        }
        public static void SetDamageMultiplier(Buff buff, float value) => buff.SetProperty(PROP_DAMAGE_MULTIPLIER, value);
        public static void SetRangeMultiplier(Buff buff, float value) => buff.SetProperty(PROP_RANGE_MULTIPLIER, value);
        public static void SetScaleMultiplier(Buff buff, Vector3 value) => buff.SetProperty(PROP_SCALE_MULTIPLIER, value);
        public static readonly VanillaBuffPropertyMeta<float> PROP_DAMAGE_MULTIPLIER = new VanillaBuffPropertyMeta<float>("damageMultiplier");
        public static readonly VanillaBuffPropertyMeta<float> PROP_RANGE_MULTIPLIER = new VanillaBuffPropertyMeta<float>("rangeMultiplier");
        public static readonly VanillaBuffPropertyMeta<Vector3> PROP_SCALE_MULTIPLIER = new VanillaBuffPropertyMeta<Vector3>("scaleMultiplier");
    }
}
