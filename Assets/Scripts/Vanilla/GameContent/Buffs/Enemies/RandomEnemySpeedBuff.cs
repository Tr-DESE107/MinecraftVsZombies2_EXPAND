using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.randomEnemySpeed)]
    public class RandomEnemySpeedBuff : BuffDefinition
    {
        public RandomEnemySpeedBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, PROP_SPEED));
        }
        public const string PROP_SPEED = "Speed";
    }
}
