using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Callbacks;

namespace PVZEngine.Grids
{
    public class GridDefinition : Definition
    {
        public GridDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual NamespaceID GetPlaceSound(Entity entity)
        {
            return null;
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.GRID;
    }
}
