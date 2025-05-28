﻿using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.theCreaturesHeart)]
    public class TheCreaturesHeart : ArtifactDefinition
    {
        public TheCreaturesHeart(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_LEVEL_START, PostLevelStartCallback);
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.PLANT);
            AddTrigger(LevelCallbacks.POST_ENTITY_REMOVE, PostEntityRemoveCallback, filter: EntityTypes.PLANT);
            AddAura(new ReduceCostAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostLevelStartCallback(LevelCallbackParams param, CallbackResult result)
        {
            var level = param.level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact == null || artifact.Definition != this)
                    continue;
                AuraEffect aura = artifact.GetAuraEffect<ReduceCostAura>();
                if (aura == null)
                    continue;
                aura.UpdateAura();
            }
        }
        private void PostEntityInitCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact == null || artifact.Definition != this)
                    continue;
                AuraEffect aura = artifact.GetAuraEffect<ReduceCostAura>();
                if (aura == null)
                    continue;
                aura.UpdateAura();
            }
        }
        private void PostEntityRemoveCallback(EntityCallbackParams param, CallbackResult result)
        {
            var contraption = param.entity;
            var level = contraption.Level;
            foreach (var artifact in level.GetArtifacts())
            {
                if (artifact == null || artifact.Definition != this)
                    continue;
                AuraEffect aura = artifact.GetAuraEffect<ReduceCostAura>();
                if (aura == null)
                    continue;
                aura.UpdateAura();
            }
        }
        public static readonly NamespaceID ID = VanillaArtifactID.theCreaturesHeart;

        public class ReduceCostAura : AuraEffectDefinition
        {
            public ReduceCostAura()
            {
                BuffID = VanillaBuffID.SeedPack.theCreaturesHeartReduceCost;
                UpdateInterval = 15;
            }
            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                foreach (var seed in level.GetAllSeedPacks())
                {
                    var seedDef = seed?.Definition;
                    if (seedDef == null)
                        continue;
                    if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                        continue;
                    var entityID = seedDef?.GetSeedEntityID();
                    if (!NamespaceID.IsValid(entityID))
                        continue;
                    var entityDef = level.Content.GetEntityDefinition(entityID);
                    if (entityDef == null || entityDef.Type != EntityTypes.PLANT)
                        continue;
                    results.Add(seed);
                }
            }
            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                if (target is not SeedPack seed)
                    return;
                var seedDef = seed?.Definition;
                if (seedDef == null)
                    return;
                if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                    return;
                var entityID = seed?.Definition?.GetSeedEntityID();
                if (!NamespaceID.IsValid(entityID))
                    return;
                buff.SetProperty(TheCreaturesHeartReduceCostBuff.PROP_ADDITION, seed.Level.GetEntityCount(entityID) * REDUCTION);
            }
            public const float REDUCTION = -5;
        }
    }
}
