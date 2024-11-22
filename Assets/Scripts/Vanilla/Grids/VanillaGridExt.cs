using System.Linq;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridExt
    {
        public static Entity GetMainEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetTakenEntities().FirstOrDefault();
        }
    }
}
