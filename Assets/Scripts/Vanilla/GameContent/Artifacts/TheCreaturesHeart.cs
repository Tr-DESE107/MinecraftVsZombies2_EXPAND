using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.SeedPacks;
using MVZ2.Vanilla;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.theCreaturesHeart)]
    public class TheCreaturesHeart : ArtifactDefinition
    {
        public TheCreaturesHeart(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_INIT, PostEntityInitCallback, filter: EntityTypes.PLANT);
            AddTrigger(LevelCallbacks.POST_ENTITY_REMOVE, PostEntityRemoveCallback, filter: EntityTypes.PLANT);
            AddAura(new ReduceCostAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostEntityInitCallback(Entity contraption)
        {
            var level = contraption.Level;
            var artifact = level.GetArtifact(ID);
            if (artifact == null)
                return;
            AuraEffect aura = artifact.GetAuraEffect<ReduceCostAura>();
            if (aura == null)
                return;
            aura.UpdateAura();
        }
        private void PostEntityRemoveCallback(Entity contraption)
        {
            var level = contraption.Level;
            var artifact = level.GetArtifact(ID);
            if (artifact == null)
                return;
            AuraEffect aura = artifact.GetAuraEffect<ReduceCostAura>();
            if (aura == null)
                return;
            aura.UpdateAura();
        }
        public static readonly NamespaceID ID = VanillaArtifactID.theCreaturesHeart;

        public class ReduceCostAura : AuraEffectDefinition
        {
            public ReduceCostAura()
            {
                BuffID = VanillaBuffID.SeedPack.theCreaturesHeartReduceCost;
                UpdateInterval = 15;
            }
            public override IEnumerable<IBuffTarget> GetAuraTargets(LevelEngine level, AuraEffect auraEffect)
            {
                foreach (var seed in level.GetAllSeedPacks())
                {
                    var entityID = seed?.Definition?.GetSeedEntityID();
                    if (!NamespaceID.IsValid(entityID))
                        continue;
                    var entityDef = level.Content.GetEntityDefinition(entityID);
                    if (entityDef == null || entityDef.Type != EntityTypes.PLANT)
                        continue;
                    yield return seed;
                }
            }
            public override void UpdateTargetBuff(AuraEffect effect, IBuffTarget target, Buff buff)
            {
                base.UpdateTargetBuff(effect, target, buff);
                if (target is not SeedPack seed)
                    return;
                var entityID = seed?.Definition?.GetSeedEntityID();
                if (!NamespaceID.IsValid(entityID))
                    return;
                buff.SetProperty(TheCreaturesHeartReduceCostBuff.PROP_ADDITION, seed.Level.FindEntities(entityID).Length * REDUCTION);
            }
            public const float REDUCTION = -10;
        }
    }
}
