using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Artifacts
{
    [Definition(VanillaArtifactNames.sweetSleepPillow)]
    public class SweetSleepPillow : ArtifactDefinition
    {
        public SweetSleepPillow(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_UPDATE, PostContraptionUpdateCallback, filter: EntityTypes.PLANT);
        }
        public override void PostUpdate(Artifact artifact)
        {
            base.PostUpdate(artifact);
            artifact.SetGlowing(true);
        }
        private void PostContraptionUpdateCallback(Entity entity)
        {
            var level = entity.Level;
            var artifact = level.GetArtifact(ID);
            if (artifact == null)
                return;
            entity.HealEffects(0.33333333f, entity);
        }
        public static readonly NamespaceID ID = VanillaArtifactID.sweetSleepPillow;
    }
}
