using MVZ2.Vanilla;
using MVZ2.Vanilla.Level;
using PVZEngine.Buffs;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Level.pagodaBranchLevel)]
    public class PagodaBranchLevelBuff : BuffDefinition
    {
        public PagodaBranchLevelBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaLevelProps.SPAWN_POINTS_MUTLIPLIER, NumberOperator.AddMultiplie, 2));
        }
    }
}
