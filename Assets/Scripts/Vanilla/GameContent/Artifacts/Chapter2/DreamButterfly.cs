using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Callbacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.GameContent.Artifacts
{
    [ArtifactDefinition(VanillaArtifactNames.dreamButterfly)]
    public class DreamButterfly : ArtifactDefinition
    {
        public DreamButterfly(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.POST_PLACE_ENTITY, PostEntityPlaceCallback);
        }
        private void PostEntityPlaceCallback(LawnGrid grid, Entity entity)
        {
            if (entity.Type != EntityTypes.PLANT)
                return;
            var level = entity.Level;
            var artifact = level.GetArtifact(ID);
            if (artifact == null)
                return;
            entity.AddBuff<DreamButterflyShieldBuff>();
            artifact.Highlight();
        }
        public static readonly NamespaceID ID = VanillaArtifactID.dreamButterfly;
    }
}
