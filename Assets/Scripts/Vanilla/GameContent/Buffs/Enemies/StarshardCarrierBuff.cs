using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.starshardCarrier)]
    public class StarshardCarrierBuff : BuffDefinition
    {
        public StarshardCarrierBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));
        }

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateColorOffset(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateColorOffset(buff);
        }
        private void UpdateColorOffset(Buff buff)
        {
            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            time %= MAX_TIME;
            var alpha = 1 - (Mathf.Cos((float)time / MAX_TIME * 360 * Mathf.Deg2Rad) + 1) / 2;
            alpha *= 0.8f;
            buff.SetProperty(PROP_COLOR_OFFSET, new Color(0, 1, 0, alpha));
            buff.SetProperty(PROP_TIME, time);
        }
        public const int MAX_TIME = 60;
        public const string PROP_TIME = "Time";
        public const string PROP_COLOR_OFFSET = "ColorOffset";
    }
}
