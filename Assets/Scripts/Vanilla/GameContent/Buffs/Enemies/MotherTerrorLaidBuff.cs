using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.motherTerrorLaid)]
    public class MotherTerrorLaidBuff : BuffDefinition
    {
        public MotherTerrorLaidBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, 0.3f));
        }
    }
}
