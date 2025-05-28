﻿using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using PVZEngine.Auras;
using PVZEngine.Buffs;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.greedyVacuum)]
    public class GreedyVacuum : ArtifactDefinition
    {
        public GreedyVacuum(string nsp, string name) : base(nsp, name)
        {
            AddAura(new Aura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        public class Aura : AuraEffectDefinition
        {
            public Aura()
            {
                BuffID = VanillaBuffID.Level.greedyVacuum;
                UpdateInterval = 15;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                results.Add(auraEffect.Level);
            }
        }
    }
}
