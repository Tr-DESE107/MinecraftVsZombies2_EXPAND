﻿using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.pagodaBranch)]
    public class PagodaBranch : ArtifactDefinition
    {
        public PagodaBranch(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
            AddAura(new LevelAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostLevelStartCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition.GetID() != VanillaArtifactID.pagodaBranch)
                    continue;
                var slotCount = level.GetStarshardSlotCount();
                var starshardCount = level.GetStarshardCount();
                if (starshardCount < slotCount)
                {
                    starshardCount = Mathf.Clamp(starshardCount + 2, 0, slotCount);
                    artifact.Highlight();
                    level.PlaySound(VanillaSoundID.starshardUse);
                }
                level.SetStarshardCount(starshardCount);
            }
        }
        public class LevelAura : AuraEffectDefinition
        {
            public LevelAura()
            {
                BuffID = VanillaBuffID.Level.pagodaBranchLevel;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                results.Add(auraEffect.Level);
            }
        }
    }
}
