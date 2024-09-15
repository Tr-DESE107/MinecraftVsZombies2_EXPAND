using MVZ2.GameContent.Enemies;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.randomEnemySpeed)]
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(BuiltinEnemyProps.SPEED, ModifyOperator.Multiply, PROP_SPEED));
        }
        public const string PROP_SPEED = "Speed";
    }
}
