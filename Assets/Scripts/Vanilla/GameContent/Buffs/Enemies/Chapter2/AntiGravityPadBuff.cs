using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Modifiers;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Enemy.AntiGravityPadGravity)]
    public class AntiGravityPadGravityBuff : BuffDefinition
    {
        public AntiGravityPadGravityBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.Add, -1.25f));
            AddModifier(new FloatModifier(VanillaEnemyProps.SPEED, NumberOperator.Multiply, 1.25f));
        }
    }
}
