using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.miracleMalletReplica)]
    public class MiracleMalletReplica : ArtifactDefinition
    {
        public MiracleMalletReplica(string nsp, string name) : base(nsp, name)
        {
            AddAura(new DamageAura());
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        public class DamageAura : AuraEffectDefinition
        {
            public DamageAura()
            {
                BuffID = VanillaBuffID.miracleMalletReplicaDamage;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                results.AddRange(level.GetEntities(EntityTypes.PLANT));
            }
        }
    }
}
