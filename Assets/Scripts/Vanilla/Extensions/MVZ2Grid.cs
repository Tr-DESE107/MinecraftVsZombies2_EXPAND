using System.Linq;
using PVZEngine.Level;

namespace MVZ2.Vanilla
{
    public static class MVZ2Grid
    {
        public static Entity GetMainEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetTakenEntities().FirstOrDefault();
        }
    }
}
