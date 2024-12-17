using System.Linq;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [Definition(VanillaBuffNames.Boss.frankensteinTransforming)]
    public class FrankensteinTransformingBuff : BuffDefinition
    {
        public FrankensteinTransformingBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new BooleanModifier(VanillaEntityProps.INVISIBLE, true));
            AddTrigger(LevelCallbacks.PRE_ENTITY_COLLISION, PreEntityCollisionCallback);
        }
        private bool PreEntityCollisionCallback(EntityCollision collision)
        {
            var entity = collision.Entity;
            var other = collision.Other;
            if (entity == null || other == null)
                return true;
            var entityBuffs = entity.HasBuff<FrankensteinTransformingBuff>();
            return !entity.HasBuff<FrankensteinTransformingBuff>() && !other.HasBuff<FrankensteinTransformingBuff>();
        }
    }
}
