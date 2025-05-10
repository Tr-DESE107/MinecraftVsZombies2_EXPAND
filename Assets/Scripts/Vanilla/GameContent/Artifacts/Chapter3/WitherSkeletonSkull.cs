using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Armors;
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
    [ArtifactDefinition(VanillaArtifactNames.witherSkeletonSkull)]
    public class WitherSkeletonSkull : ArtifactDefinition
    {
        public WitherSkeletonSkull(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostContraptionInitCallback, filter: EntityTypes.PLANT);
            AddAura(new ReduceCostAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostContraptionInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            var artifacts = level.GetArtifacts();
            foreach (var artifact in artifacts)
            {
                if (artifact == null)
                    continue;
                if (artifact.Definition != this)
                    continue;
                artifact.Highlight();
                contraption.AddBuff<WitherSkeletonSkullReduceHealthBuff>();
            }
        }
        public static readonly NamespaceID ID = VanillaArtifactID.theCreaturesHeart;

        public class ReduceCostAura : AuraEffectDefinition
        {
            public ReduceCostAura()
            {
                BuffID = VanillaBuffID.SeedPack.witherSkeletonSkullReduceCost;
                UpdateInterval = 4;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                foreach (var seed in level.GetAllSeedPacks())
                {
                    var seedDef = seed?.Definition;
                    if (seedDef == null)
                        continue;
                    results.Add(seed);
                }
            }
        }
    }
}
