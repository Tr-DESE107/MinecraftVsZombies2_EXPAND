using MVZ2.Managers;
using MVZ2.UI;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Games;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Models
{
    public class ArtifactPickupModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var artifactID = Model.GetProperty<NamespaceID>("ArtifactID");
            if (lastID != artifactID)
            {
                lastID = artifactID;
                var artifactDef = Main.Game.GetArtifactDefinition(artifactID);
                if (artifactDef == null)
                    return;
                var sprRef = artifactDef.GetSpriteReference();
                artifactSprite.sprite = Main.GetFinalSprite(sprRef);
            }
        }
        [SerializeField]
        private SpriteRenderer artifactSprite;
        private NamespaceID lastID;
    }
}
