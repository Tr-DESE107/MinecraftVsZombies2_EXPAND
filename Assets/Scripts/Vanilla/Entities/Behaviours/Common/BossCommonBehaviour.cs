﻿using MVZ2.GameContent.Damages;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;

namespace MVZ2.Vanilla.Entities
{
    [EntityBehaviourDefinition(VanillaEntityBehaviourNames.bossCommon)]
    public class BossCommonBehaviour : EntityBehaviourDefinition
    {
        public BossCommonBehaviour(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostTakeDamage(DamageOutput result)
        {
            base.PostTakeDamage(result);
            var bodyResult = result.BodyResult;
            if (bodyResult != null && bodyResult.Amount > 0 && !bodyResult.HasEffect(VanillaDamageEffects.NO_DAMAGE_BLINK))
            {
                var entity = bodyResult.Entity;
                entity.DamageBlink();
            }
        }
    }
}
