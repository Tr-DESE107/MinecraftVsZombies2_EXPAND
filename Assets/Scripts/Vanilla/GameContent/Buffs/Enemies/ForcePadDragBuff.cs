using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.forcePadDrag)]
    public class ForcePadDragBuff : BuffDefinition
    {
        public ForcePadDragBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new Vector3Modifier(EngineEntityProps.VELOCITY_DAMPEN, NumberOperator.ForceSet, Vector3.one));
            AddModifier(new FloatModifier(EngineEntityProps.GRAVITY, NumberOperator.ForceSet, 0));
        }
    }
}
