using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Triggers;

namespace PVZEngine.Grids
{
    public class GridDefinition : Definition
    {
        public GridDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public sealed override string GetDefinitionType() => EngineDefinitionTypes.GRID;
    }
}
