using PVZEngine;
using PVZEngine.Base;

namespace MVZ2Logic.Errors
{
    public class GridLayerDefinition : Definition
    {
        public GridLayerDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public sealed override string GetDefinitionType() => LogicDefinitionTypes.GRID_LAYER;
        public NamespaceID AlmanacTag { get; set; }
    }
}
