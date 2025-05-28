﻿using System.Collections.Generic;
using System.Linq;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Modifiers;

namespace PVZEngine.Buffs
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
        public PropertyModifier[] GetModifiers(IPropertyKey propName)
        {
            return modifiers.Where(e => e.PropertyName == propName).ToArray();
        }
        public ModelInsertion[] GetModelInsertions()
        {
            return modelInsertions.ToArray();
        }
        public AuraEffectDefinition[] GetAuras()
        {
            return auraDefinitions.ToArray();
        }
        public virtual void PostAdd(Buff buff) { }
        public virtual void PostRemove(Buff buff) { }
        public virtual void PostUpdate(Buff buff) { }
        protected void AddModifier(PropertyModifier modifier)
        {
            modifiers.Add(modifier);
        }
        protected void AddModelInsertion(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            modelInsertions.Add(new ModelInsertion(anchorName, key, modelID));
        }
        protected void AddAura(AuraEffectDefinition aura)
        {
            auraDefinitions.Add(aura);
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.BUFF;
        private List<PropertyModifier> modifiers = new List<PropertyModifier>();
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
        private List<ModelInsertion> modelInsertions = new List<ModelInsertion>();
    }
}
