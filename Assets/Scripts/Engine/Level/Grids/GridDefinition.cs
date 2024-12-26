using PVZEngine.Base;
using PVZEngine.Triggers;

namespace PVZEngine.Grids
{
    public class GridDefinition : Definition
    {
        public GridDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void CanPlaceEntity(LawnGrid grid, NamespaceID entityID, TriggerResultNamespaceID error)
        {
        }
    }
}
