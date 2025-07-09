﻿using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.dreamKey)]
    public class DreamKey : ArtifactDefinition
    {
        public DreamKey(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_CONTRAPTION_EVOKE, PostContraptionEvokeCallback);
            AddAura(new EvokedContraptionInvincibleAura());
        }
        private void PostContraptionEvokeCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            if (!level.HasArtifact(ID))
                return;
            contraption.HealEffects(contraption.GetMaxHealth(), (Entity)null);
            var artifact = level.GetArtifact(ID);
            artifact.Highlight();
        }
        public static readonly NamespaceID ID = VanillaArtifactID.dreamKey;

        public class EvokedContraptionInvincibleAura : AuraEffectDefinition
        {
            public EvokedContraptionInvincibleAura()
            {
                BuffID = VanillaBuffID.dreamKeyShield;
                UpdateInterval = 7;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.FindEntities(e => e.Type == EntityTypes.PLANT && e.IsEvoked()));
            }
        }
    }
}
