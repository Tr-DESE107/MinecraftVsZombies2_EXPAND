using System.Collections.Generic;
using PVZEngine.Armors;
using PVZEngine.Auras;
using PVZEngine.Base;
using PVZEngine.Definitions;

namespace PVZEngine.Entities
{
    public abstract class ArmorBehaviourDefinition : Definition
    {
        protected ArmorBehaviourDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void PostUpdate(Armor armor) { }
        public AuraEffectDefinition[] GetAuras()
        {
            return auraDefinitions.ToArray();
        }
        protected void AddAura(AuraEffectDefinition aura)
        {
            auraDefinitions.Add(aura);
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.ARMOR_BEHAVIOUR;
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
    }
}
