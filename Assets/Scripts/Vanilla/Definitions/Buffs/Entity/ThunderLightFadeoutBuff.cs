using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{

    [Definition(VanillaBuffNames.thunderLightFadeout)]
    public class ThunderLightFadeoutBuff : BuffDefinition
    {
        public ThunderLightFadeoutBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(BuiltinEntityProps.LIGHT_COLOR, PROP_COLOR_MULTIPLIER));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateMultiplier(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateMultiplier(buff);
        }
        private void UpdateMultiplier(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null)
                return;
            var alpha = Mathf.Clamp01(entity.Timeout / 30f);
            var color = new Color(1, 1, 1, alpha);
            buff.SetProperty(PROP_COLOR_MULTIPLIER, color);
        }
        public const string PROP_COLOR_MULTIPLIER = "ColorMultiplier";
    }
}
