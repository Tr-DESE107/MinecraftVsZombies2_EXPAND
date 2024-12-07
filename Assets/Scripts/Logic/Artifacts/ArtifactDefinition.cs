using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;

namespace MVZ2Logic.Artifacts
{
    public abstract class ArtifactDefinition : Definition
    {
        public ArtifactDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public abstract SpriteReference GetSpriteReference();
        public abstract NamespaceID GetBuffID();
    }
}
