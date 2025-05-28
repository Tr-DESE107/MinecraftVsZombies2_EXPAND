﻿using PVZEngine;

namespace MVZ2Logic.Artifacts
{
    [PropertyRegistryRegion(PropertyRegions.artifact)]
    public static class LogicArtifactProps
    {
        public static readonly PropertyMeta<SpriteReference> SPRITE_REFERENCE = new PropertyMeta<SpriteReference>("spriteReference");
        public static readonly PropertyMeta<int> NUMBER = new PropertyMeta<int>("number");
        public static readonly PropertyMeta<bool> INACTIVE = new PropertyMeta<bool>("inactive");
        public static readonly PropertyMeta<bool> GLOWING = new PropertyMeta<bool>("glowing");
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
        public static readonly PropertyMeta<NamespaceID> TRANSFORM_SOURCE = new PropertyMeta<NamespaceID>("transformSource");
        public static void SetTransformSource(this Artifact artifact, NamespaceID id)
        {
            artifact.SetProperty(TRANSFORM_SOURCE, id);
        }
        public static NamespaceID GetTransformSource(this Artifact artifact)
        {
            return artifact.GetProperty<NamespaceID>(TRANSFORM_SOURCE);
        }
        public static readonly PropertyMeta<NamespaceID> UNLOCK_ID = new PropertyMeta<NamespaceID>("unlockID");
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

