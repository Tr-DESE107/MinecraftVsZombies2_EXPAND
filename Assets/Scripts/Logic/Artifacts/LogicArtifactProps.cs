namespace MVZ2Logic.Artifacts
{
    public static class LogicArtifactProps
    {
        public const string SPRITE_REFERENCE = "spriteReference";
        public const string NUMBER = "number";
        public const string INACTIVE = "inactive";
        public const string GLOWING = "glowing";
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
    }
}
