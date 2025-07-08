using System.Collections.Generic;
using PVZEngine.Buffs;

namespace PVZEngine.Auras
{
    public abstract class AuraEffectDefinition
    {
        public AuraEffectDefinition()
        {
        }
        public AuraEffectDefinition(NamespaceID buffID, int updateInterval = 1)
        {
            BuffID = buffID;
            UpdateInterval = updateInterval;
        }
        public virtual void PostAdd(AuraEffect auraEffect) { }
        public virtual void PostRemove(AuraEffect auraEffect) { }
        public abstract void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results);
        public virtual void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff) { }
        public NamespaceID BuffID { get; protected set; }
        public int UpdateInterval { get; protected set; } = 1;
    }
}
