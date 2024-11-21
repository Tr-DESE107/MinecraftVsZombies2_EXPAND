using MVZ2.Vanilla;
using MVZ2Logic.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs
{
    [Definition(VanillaBuffNames.randomEnemySpeed)]
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(BuiltinEnemyProps.SPEED, NumberOperator.Multiply, PROP_SPEED));
        }
        public const string PROP_SPEED = "Speed";
    }
}
