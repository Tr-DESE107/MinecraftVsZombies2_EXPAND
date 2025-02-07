using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using PVZEngine.Triggers;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.Boss.frankensteinTransforming)]
    public class FrankensteinTransformingBuff : BuffDefinition
    {
        public FrankensteinTransformingBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.INVISIBLE, true));
            AddTrigger(LevelCallbacks.PRE_ENTITY_COLLISION, PreEntityCollisionCallback);
        }
        private void PreEntityCollisionCallback(EntityCollision collision, TriggerResultBoolean result)
        {
            var entity = collision.Entity;
            var other = collision.Other;
            if (entity == null || other == null)
                return;
            var entityBuffs = entity.HasBuff<FrankensteinTransformingBuff>();
            result.Result = !entity.HasBuff<FrankensteinTransformingBuff>() && !other.HasBuff<FrankensteinTransformingBuff>();
            result.Interrupt();
        }
    }
}
