using PVZEngine;

namespace MVZ2Logic.Artifacts
{
    public class ArtifactChooseItem
    {
        public ArtifactChooseItem(NamespaceID id, bool innate = false)
        {
            this.id = id;
            this.innate = innate;
        }
        public NamespaceID id;
        public bool innate;

    }
}
