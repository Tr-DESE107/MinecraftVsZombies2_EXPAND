using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.darkMatterDark)]
    public class DarkMatterDarkBuff : BuffDefinition
    {
        public DarkMatterDarkBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaAreaProps.DARKNESS_VALUE, NumberOperator.Add, PROP_DARKNESS_ADDITION));
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var addition = buff.GetProperty<float>(PROP_DARKNESS_ADDITION);
            addition = Mathf.Clamp(addition + DARKNESS_SPEED, 0, MAX_DARKNESS);
            buff.SetProperty(PROP_DARKNESS_ADDITION, addition);
        }
        public const string PROP_DARKNESS_ADDITION = "DarknessAddition";
        public const float DARKNESS_SPEED = 0.03f;
        public const float MAX_DARKNESS = 0.9f;
    }
}
