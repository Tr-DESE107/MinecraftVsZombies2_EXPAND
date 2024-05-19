using MVZ2.Vanilla.Modifiers;
using PVZEngine;

namespace MVZ2.Vanilla.Buffs
{
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff()
        {
            AddModifier(new IntModifier(EnemyProps.SPEED, ModifyOperator.Multiply, PROP_SPEED));
        }
        public const string PROP_SPEED = "Speed";
    }
}
