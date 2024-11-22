using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
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
