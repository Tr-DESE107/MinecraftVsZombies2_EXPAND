using PVZEngine.Auras;
using System.Collections.Generic;
using PVZEngine.Base;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEditor.Experimental;

namespace PVZEngine.Definitions
{
    public class SeedDefinition : Definition
    {
        public SeedDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineSeedProps.RECHARGE_SPEED, 1);
        }
        public virtual void Update(SeedPack seedPack, float rechargeSpeed) { }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.SEED;
        public int GetAuraCount()
        {
            return auraDefinitions.Count;
        }
        public AuraEffectDefinition GetAuraAt(int index)
        {
            return auraDefinitions[index];
        }
        protected void AddAura(AuraEffectDefinition aura)
        {
            auraDefinitions.Add(aura);
        }
        private List<AuraEffectDefinition> auraDefinitions = new List<AuraEffectDefinition>();
    }
}
