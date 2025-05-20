using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.theGiantPacman)]
    public class TheGiantPacmanBuff : BuffDefinition
    {
        public TheGiantPacmanBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.SIZE, NumberOperator.Set, Vector3.one * 240));
        }
    }
}
