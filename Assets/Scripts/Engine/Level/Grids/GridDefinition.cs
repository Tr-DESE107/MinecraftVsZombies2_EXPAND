using PVZEngine.Base;

namespace PVZEngine.Grids
{
    public class GridDefinition : Definition
    {
        public GridDefinition(string nsp, string name) : base(nsp, name)
        {
        }
        public virtual void CanPlaceEntity(LawnGrid grid, NamespaceID entityID, GridStatus data)
        {
        }
    }
    public class GridStatus
    {
        public NamespaceID Error { get; set; }
    }
}
