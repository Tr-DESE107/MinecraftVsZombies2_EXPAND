using System.Collections.Generic;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    public interface ICarrierBehaviour
    {
    }
    public abstract class CarrierContraptionBehaviour : EntityBehaviourDefinition, ICarrierBehaviour
    {
        public CarrierContraptionBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddAura(new PassengerAura(GetPassenagerBuffID()));
            AddAura(new CarrierAura(GetCarrierBuffID()));
        }
        protected abstract NamespaceID GetCarrierBuffID();
        protected abstract NamespaceID GetPassenagerBuffID();
        private class CarrierAura : AuraEffectDefinition
        {
            public CarrierAura(NamespaceID buffID)
            {
                BuffID = buffID;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var sourceEnt = auraEffect.Source.GetEntity();
                if (sourceEnt != null && sourceEnt.HasPassenger())
                {
                    results.Add(sourceEnt);
                }
            }
        }
        private class PassengerAura : AuraEffectDefinition
        {
            public PassengerAura(NamespaceID buffID)
            {
                BuffID = buffID;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var sourceEnt = auraEffect.Source.GetEntity();
                var grid = sourceEnt?.GetGrid();
                if (grid != null)
                {
                    results.Add(grid.GetMainEntity());
                    results.Add(grid.GetProtectorEntity());
                }
            }
        }
    }
}
