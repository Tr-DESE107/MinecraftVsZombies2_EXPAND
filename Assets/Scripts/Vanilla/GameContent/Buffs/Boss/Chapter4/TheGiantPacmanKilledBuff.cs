using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.theGiantPacmanKilled)]
    public class TheGiantPacmanKilledBuff : BuffDefinition
    {
        public TheGiantPacmanKilledBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(EngineEntityProps.COLLISION_DETECTION, NumberOperator.Set, EntityCollisionHelper.DETECTION_IGNORE));
            AddModifier(new Vector3Modifier(EngineEntityProps.SCALE, NumberOperator.Multiply, Vector3.zero));
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
            AddModifier(new BooleanModifier(VanillaEntityProps.INVISIBLE, true));
        }
    }
}
