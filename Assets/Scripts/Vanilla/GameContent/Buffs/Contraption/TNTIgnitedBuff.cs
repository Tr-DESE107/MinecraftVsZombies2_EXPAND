using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [Definition(VanillaBuffNames.tntIgnited)]
    public class TNTIgnitedBuff : BuffDefinition
    {
        public TNTIgnitedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR));
        }
        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            buff.SetProperty(PROP_TIME, 0);
            UpdateMultipliers(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateMultipliers(buff);

            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            buff.SetProperty(PROP_TIME, time);
        }
        private void UpdateMultipliers(Buff buff)
        {
            var time = buff.GetProperty<int>(PROP_TIME);
            var alpha = (Mathf.Sin(time * 24 * Mathf.Deg2Rad) + 1) * 0.5f;
            buff.SetProperty(PROP_COLOR, new Color(1, 1, 1, alpha));
        }
        public const string PROP_TIME = "Time";
        public const string PROP_COLOR = "Color";
    }
}
