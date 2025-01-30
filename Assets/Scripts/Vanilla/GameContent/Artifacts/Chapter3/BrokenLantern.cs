using System.Collections.Generic;
using MVZ2.GameContent.Buffs;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Artifacts;
using PVZEngine.Auras;
using PVZEngine.Buffs;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.brokenLantern)]
    public class BrokenLantern : ArtifactDefinition
    {
        public BrokenLantern(string nsp, string name) : base(nsp, name)
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
                BuffID = VanillaBuffID.brokenLantern;
                UpdateInterval = 4;
            }

            public override void GetAuraTargets(AuraEffect auraEffect, List<IBuffTarget> results)
            {
                var level = auraEffect.Source.GetLevel();
                updateBuffer.Clear();
                level.FindEntitiesNonAlloc(e => e.IsLightSource() && e.Type == EntityTypes.PLANT, updateBuffer);
                foreach (var entity in updateBuffer)
                {
                    results.Add(entity);
                }
            }
            private List<Entity> updateBuffer = new List<Entity>();
        }
    }
}
