﻿using MVZ2.GameContent.Damages;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.GameContent.Buffs.Contraptions
{
    [BuffDefinition(VanillaBuffNames.stoneShieldProtected)]
    public class StoneShieldProtected : BuffDefinition
    {
        public StoneShieldProtected(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.PRE_ENTITY_TAKE_DAMAGE, PreEntityTakeDamageCallback, priority: -100);
        }
        private void PreEntityTakeDamageCallback(VanillaLevelCallbacks.PreTakeDamageParams param, CallbackResult result)
        {
            var damageInfo = param.input;
            var entity = damageInfo.Entity;
            if (!entity.HasBuff<StoneShieldProtected>())
                return;
            if (damageInfo.Effects.HasEffect(VanillaDamageEffects.EXPLOSION))
            {
                result.SetFinalValue(false);
            }
        }
    }
}
