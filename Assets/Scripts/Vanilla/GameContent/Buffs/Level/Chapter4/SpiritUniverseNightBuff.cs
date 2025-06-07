using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Level
{
    [BuffDefinition(VanillaBuffNames.Level.spiritUniverseNight)]
    public class SpiritUniverseNightBuff : BuffDefinition
    {
        public SpiritUniverseNightBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(ColorModifier.Multiply(VanillaAreaProps.BACKGROUND_LIGHT, PROP_BACKGROUND_LIGHT_MULTIPLIER));
        }
        public static void SetBackgroundLightMultiplier(Buff buff, Color color)
        {
            buff.SetProperty<Color>(PROP_BACKGROUND_LIGHT_MULTIPLIER, color);
        }
        public const int MAX_TIMEOUT = 60;
        public static readonly VanillaBuffPropertyMeta<Color> PROP_BACKGROUND_LIGHT_MULTIPLIER = new VanillaBuffPropertyMeta<Color>("background_light_multiplier");
    }
}
