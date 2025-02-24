using PVZEngine;

namespace MVZ2Logic.Artifacts
{
    [PropertyRegistryRegion]
    public static class LogicArtifactProps
    {
        public static readonly PropertyMeta SPRITE_REFERENCE = new PropertyMeta("spriteReference");
        public static readonly PropertyMeta NUMBER = new PropertyMeta("number");
        public static readonly PropertyMeta INACTIVE = new PropertyMeta("inactive");
        public static readonly PropertyMeta GLOWING = new PropertyMeta("glowing");
        public static SpriteReference GetSpriteReference(this ArtifactDefinition definition)
        {
            return definition.GetProperty<SpriteReference>(SPRITE_REFERENCE);
        }
        public static void SetSpriteReference(this ArtifactDefinition definition, SpriteReference spriteReference)
        {
            definition.SetProperty(SPRITE_REFERENCE, spriteReference);
        }
        public static int GetNumber(this Artifact artifact)
        {
            return artifact.GetProperty<int>(NUMBER);
        }
        public static void SetNumber(this Artifact artifact, int number)
        {
            artifact.SetProperty(NUMBER, number);
        }
        public static void SetInactive(this Artifact artifact, bool value)
        {
            artifact.SetProperty(INACTIVE, value);
        }
        public static bool IsInactive(this Artifact artifact)
        {
            return artifact.GetProperty<bool>(INACTIVE);
        }
        public static void SetGlowing(this Artifact artifact, bool value)
        {
            artifact.SetProperty(GLOWING, value);
        }
        public static bool GetGlowing(this Artifact artifact)
        {
            return artifact.GetProperty<bool>(GLOWING);
        }
        public static readonly PropertyMeta UNLOCK_ID = new PropertyMeta("unlockID");
        public static void SetUnlockID(this ArtifactDefinition artifact, NamespaceID id)
        {
            artifact.SetProperty(UNLOCK_ID, id);
        }
        public static NamespaceID GetUnlockID(this ArtifactDefinition artifact)
        {
            return artifact.GetProperty<NamespaceID>(UNLOCK_ID);
        }
    }
}
