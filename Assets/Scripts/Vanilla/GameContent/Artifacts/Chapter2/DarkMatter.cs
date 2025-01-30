using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.darkMatter)]
    public class DarkMatter : ArtifactDefinition
    {
        public DarkMatter(string nsp, string name) : base(nsp, name)
        {
            AddAura(new ProductionAura());
            AddAura(new DarkAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        public static readonly NamespaceID ID = VanillaArtifactID.darkMatter;
        public class ProductionAura : AuraEffectDefinition
        {
            public ProductionAura()
            {
                BuffID = VanillaBuffID.darkMatterProduction;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.PLANT));
            }
        }
        public class DarkAura : AuraEffectDefinition
        {
            public DarkAura()
            {
                BuffID = VanillaBuffID.Level.darkMatterDark;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                results.Add(auraEffect.Source.GetLevel());
            }
        }
    }
}
