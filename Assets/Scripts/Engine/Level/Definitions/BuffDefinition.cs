using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Level;
using PVZEngine.Level.Triggers;
using PVZEngine.Modifiers;

namespace PVZEngine.Definitions
{
    public abstract class BuffDefinition : Definition
    {
        public BuffDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public PropertyModifier[] GetModifiers()
        {
            return modifiers.ToArray();
        }
        public PropertyModifier[] GetModifiers(string propName)
        {
            return modifiers.Where(e => e.PropertyName == propName).ToArray();
        }
        public AuraEffectDefinition[] GetAuras()
        {
            return auraDefinitions.ToArray();
        }
        public TriggerCache[] GetTriggerCaches()
        {
            return triggerCaches.GetTriggerCaches();
        }
        public virtual void PostAdd(Buff buff) { }
        public virtual void PostRemove(Buff buff) { }
        public virtual void PostUpdate(Buff buff) { }
        protected void AddModifier(PropertyModifier modifier)
        {
            modifiers.Add(modifier);
        }
        protected void AddAura(AuraEffectDefinition aura)
        {
            auraDefinitions.Add(aura);
        }
        public void AddTrigger<T>(NamespaceID callbackID, T action, int priority = 0, object filterValue = null) where T : Delegate
        {
            triggerCaches.Add(new TriggerCache(callbackID, action, priority, filterValue));
        }
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
        protected TriggerCacheList triggerCaches = new TriggerCacheList();
    }
}
