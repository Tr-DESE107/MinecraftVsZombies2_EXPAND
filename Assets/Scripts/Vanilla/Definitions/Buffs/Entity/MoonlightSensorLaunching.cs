using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.moonlightSensorLaunching)]
    public class MoonlightSensorLaunchingBuff : BuffDefinition
    {
        public MoonlightSensorLaunchingBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.PRODUCE_SPEED, NumberOperator.Multiply, PRODUCE_SPEED_MULTIPLIER));
        }
        public const float PRODUCE_SPEED_MULTIPLIER = 0.625f;
    }
}
