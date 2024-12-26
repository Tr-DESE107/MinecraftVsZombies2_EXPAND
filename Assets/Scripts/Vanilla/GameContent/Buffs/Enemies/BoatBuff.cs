using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.boat)]
    public class BoatBuff : BuffDefinition
    {
        public BoatBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(VanillaEntityProps.WATER_INTERACTION, NumberOperator.Set, WaterInteraction.FLOAT));
        }
    }
}
