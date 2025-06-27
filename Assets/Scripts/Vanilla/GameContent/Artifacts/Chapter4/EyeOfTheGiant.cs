﻿using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.eyeOfTheGiant)]
    public class EyeOfTheGiant : ArtifactDefinition
    {
        public EyeOfTheGiant(string nsp, string name) : base(nsp, name)
        {
            AddAura(new EyeOfTheGiantAura());
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.PLANT);
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact == null || artifact.Definition != this)
                    continue;
                AuraEffect aura = artifact.GetAuraEffect<EyeOfTheGiantAura>();
                if (aura == null)
                    continue;
                aura.UpdateAura();
            }
        }
        public class EyeOfTheGiantAura : AuraEffectDefinition
        {
            public EyeOfTheGiantAura()
            {
                BuffID = VanillaBuffID.eyeOfTheGiant;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.PLANT));
            }
        }
    }
}
