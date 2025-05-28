using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.eyeOfTheGiant)]
    public class EyeOfTheGiant : ArtifactDefinition
    {
        public EyeOfTheGiant(string nsp, string name) : base(nsp, name)
        {
            AddAura(new EyeOfTheGiantAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
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
