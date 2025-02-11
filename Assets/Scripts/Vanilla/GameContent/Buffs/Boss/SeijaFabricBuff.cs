using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.seijaFabric)]
    public class SeijaFabricBuff : BuffDefinition
    {
        public SeijaFabricBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new IntModifier(EngineEntityProps.COLLISION_DETECTION, NumberOperator.ForceSet, EntityCollisionHelper.DETECTION_IGNORE));
            AddModifier(new Vector3Modifier(EngineEntityProps.VELOCITY_DAMPEN, NumberOperator.ForceSet, Vector3.one));
            AddModifier(new BooleanModifier(EngineEntityProps.INVINCIBLE, true));
        }
    }
}
