﻿using System.Collections.Generic;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Contraptions
{
    public interface ICarrierBehaviour
    {
        void UpdateCarrier(Entity entity);
    }
    public abstract class CarrierContraptionBehaviour : EntityBehaviourDefinition, ICarrierBehaviour
    {
        public CarrierContraptionBehaviour(string nsp, string name) : base(nsp, name)
        {
            AddAura(new PassengerAura(GetPassenagerBuffID()));
            AddAura(new CarrierAura(GetCarrierBuffID()));
        }
        public void UpdateCarrier(Entity carrier)
        {
            foreach (var aura in carrier.GetAuraEffects())
            {
                aura.UpdateAura();
            }
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
                if (sourceEnt == null)
                    return;
                var grids = sourceEnt.GetGridsToTake();
                foreach (var grid in grids)
                {
                    results.Add(grid.GetMainEntity());
                    results.Add(grid.GetProtectorEntity());
                }
            }
        }
    }

}
