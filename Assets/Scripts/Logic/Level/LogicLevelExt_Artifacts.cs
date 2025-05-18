using System.Linq;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Games;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public static partial class LogicLevelExt
    {
        public static IArtifactComponent GetArtifactComponent(this LevelEngine level)
        {
            return level.GetComponent<IArtifactComponent>();
        }
        public static void SetArtifactSlotCount(this LevelEngine level, int count)
        {
            var component = level.GetArtifactComponent();
            component.SetSlotCount(count);
        }
        public static int GetArtifactSlotCount(this LevelEngine level)
        {
            var component = level.GetArtifactComponent();
            return component.GetSlotCount();
        }
        public static void ReplaceArtifacts(this LevelEngine level, ArtifactDefinition[] definitions)
        {
            var component = level.GetArtifactComponent();
            component.ReplaceArtifacts(definitions);
        }
        public static Artifact[] GetArtifacts(this LevelEngine level)
        {
            var component = level.GetArtifactComponent();
            return component.GetArtifacts();
        }
        public static bool HasArtifact(this LevelEngine level, NamespaceID artifactID)
        {
            var component = level.GetArtifactComponent();
            return component.HasArtifact(artifactID);
        }
        public static int GetArtifactIndex(this LevelEngine level, NamespaceID artifactID)
        {
            var component = level.GetArtifactComponent();
            return component.GetArtifactIndex(artifactID);
        }
        public static Artifact GetArtifactAt(this LevelEngine level, int index)
        {
            var component = level.GetArtifactComponent();
            return component.GetArtifactAt(index);
        }
        public static void ReplaceArtifacts(this LevelEngine level, NamespaceID[] idList)
        {
            ArtifactDefinition[] definitions;
            if (idList == null)
            {
                definitions = null;
            }
            else
            {
                definitions = idList.Select(id => NamespaceID.IsValid(id) ? level.Content.GetArtifactDefinition(id) : null).ToArray();
            }
            level.ReplaceArtifacts(definitions);
        }
        public static Artifact GetArtifact(this LevelEngine level, NamespaceID artifactID)
        {
            var index = level.GetArtifactIndex(artifactID);
            if (index < 0)
                return null;
            return level.GetArtifactAt(index);
        }
    }
}
