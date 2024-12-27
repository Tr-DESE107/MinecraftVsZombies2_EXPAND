using System.Collections.Generic;
using PVZEngine.Buffs;
using PVZEngine.Level;

namespace PVZEngine.Auras
{
    public abstract class AuraEffectDefinition
    {
        public AuraEffectDefinition()
        {
        }
        public virtual void PostAdd(AuraEffect auraEffect) { }
        public virtual void PostRemove(AuraEffect auraEffect) { }
        public abstract IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect);
        public virtual void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff) { }
        public NamespaceID BuffID { get; protected set; }
        public int UpdateInterval { get; protected set; } = 1;
    }
}
