using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.theGiantPhase3)]
    public class TheGiantPhase3Buff : BuffDefinition
    {
        public TheGiantPhase3Buff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new MaxHealthModifier(NumberOperator.Multiply, 0.4f));
        }
    }
}
