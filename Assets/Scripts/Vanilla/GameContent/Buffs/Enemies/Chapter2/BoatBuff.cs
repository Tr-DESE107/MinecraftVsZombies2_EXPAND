using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.Modifiers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.boat)]
    public class BoatBuff : BuffDefinition
    {
        public BoatBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(VanillaEntityProps.WATER_INTERACTION, NumberOperator.Set, WaterInteraction.FLOAT));
            AddModifier(new IntModifier(VanillaEntityProps.AIR_INTERACTION, NumberOperator.Set, WaterInteraction.FLOAT));
        }
    }
}
