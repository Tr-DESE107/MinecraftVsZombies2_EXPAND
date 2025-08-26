using MVZ2.GameContent.Artifacts;
using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Pickups
{
    [EntityBehaviourDefinition(VanillaPickupNames.artifactPickup)]
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
            var artifactID = GetArtifactID(pickup);
            var saves = Global.Saves;
            if (!saves.IsArtifactUnlocked(artifactID))
            {
                var unlockID = VanillaArtifactID.GetUnlockID(artifactID);
                saves.Unlock(unlockID);
                saves.SaveToFile(); // 获得制品后保存游戏。
                level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_YOU_FOUND_A_NEW_ARTIFACT, 0, 150);
            }
            level.PlaySound(pickup.GetCollectSound());
            level.Spawn(VanillaEffectID.starParticles, pickup.Position, pickup);
            pickup.Remove();
        }
        public static NamespaceID GetArtifactID(Entity pickup) => pickup.GetPickupContentID();
        public static void SetArtifactID(Entity pickup, NamespaceID id) => pickup.SetPickupContentID(id);
        private static readonly NamespaceID ID = VanillaPickupID.artifactPickup;
    }
}