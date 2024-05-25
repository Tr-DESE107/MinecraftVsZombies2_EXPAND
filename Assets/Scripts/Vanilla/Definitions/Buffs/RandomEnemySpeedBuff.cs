using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Modifiers;
using MVZ2.Vanilla;
using PVZEngine;

namespace MVZ2.GameContent.Buffs
{
    [Definition(BuffNames.randomEnemySpeed)]
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff()
        {
            AddModifier(new FloatModifier(EnemyProps.SPEED, ModifyOperator.Multiply, PROP_SPEED));
        }
        public const string PROP_SPEED = "Speed";
    }
}
