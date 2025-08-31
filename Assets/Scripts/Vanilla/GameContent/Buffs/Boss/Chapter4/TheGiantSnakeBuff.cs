using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.theGiantSnake)]
    public class TheGiantSnakeBuff : BuffDefinition
    {
        public TheGiantSnakeBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SIZE, NumberOperator.Set, Vector3.one * 80));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_OFFSET, NumberOperator.Set, Vector3.zero));
            AddModifier(new Vector3Modifier(VanillaEntityProps.SHADOW_SCALE, NumberOperator.Set, Vector3.one * 2));
        }
    }
}
