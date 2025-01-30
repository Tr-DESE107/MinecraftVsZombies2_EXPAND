using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [Definition(VanillaPickupNames.artifactPickup)]
    public class ArtifactPickup : PickupBehaviour
    {
        public ArtifactPickup(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostContactGround(Entity entity, Vector3 velocity)
        {
            base.PostContactGround(entity, velocity);
            entity.Velocity = Vector3.zero;
        }
        public override void PostCollect(Entity pickup)
        {
            base.PostCollect(pickup);
            var level = pickup.Level;
            var unlockID = GetArtifactUnlockID(pickup);
            if (NamespaceID.IsValid(unlockID) && !Global.Game.IsUnlocked(unlockID))
            {
                Global.Game.Unlock(unlockID);
                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_YOU_FOUND_A_NEW_ARTIFACT, 0, 150);
            }
            level.PlaySound(pickup.GetCollectSound());
            level.Spawn(VanillaEffectID.starParticles, pickup.Position, pickup);
            pickup.Remove();
        }
        public static NamespaceID GetArtifactUnlockID(Entity pickup)
        {
            var artifactID = GetArtifactID(pickup);
            var artifactDefinition = pickup.Level.Content.GetArtifactDefinition(artifactID);
            if (artifactDefinition == null)
                return null;
            return artifactDefinition.GetUnlockID();
        }
        public static NamespaceID GetArtifactID(Entity pickup) => pickup.GetBehaviourField<NamespaceID>(ID, FIELD_ARTIFACT_ID);
        public static void SetArtifactID(Entity pickup, NamespaceID id)
        {
            pickup.SetBehaviourField(ID, FIELD_ARTIFACT_ID, id);
            pickup.SetModelProperty("ArtifactID", id);
        }
        public const string FIELD_ARTIFACT_ID = "ArtifactID";
        private static readonly NamespaceID ID = VanillaPickupID.artifactPickup;
    }
}