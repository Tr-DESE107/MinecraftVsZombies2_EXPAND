using MVZ2.GameContent;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace MVZ2.Vanilla
{
    [Definition(VanillaBuffNames.hardEnemy)]
    public class HardEnemyBuff : BuffDefinition
    {
        public HardEnemyBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, 1.5f));
        }
    }
}
