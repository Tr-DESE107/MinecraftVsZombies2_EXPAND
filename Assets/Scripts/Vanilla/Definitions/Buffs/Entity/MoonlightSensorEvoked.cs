using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.moonlightSensorEvoked)]
    public class MoonlightSensorEvokedBuff : BuffDefinition
    {
        public MoonlightSensorEvokedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.PRODUCE_SPEED, ModifyOperator.Multiply, PRODUCE_SPEED_MULTIPLIER));
            AddModifier(new BooleanModifier(BuiltinEntityProps.IS_LIGHT_SOURCE, ModifyOperator.Set, true));
            AddModifier(new Vector3Modifier(BuiltinEntityProps.LIGHT_RANGE, ModifyOperator.Add, LIGHT_RANGE_ADDITION));
        }
        public const float PRODUCE_SPEED_MULTIPLIER = 2.5f;
        public static readonly Vector3 LIGHT_RANGE_ADDITION = new Vector3(240, 240, 240);
    }
}
