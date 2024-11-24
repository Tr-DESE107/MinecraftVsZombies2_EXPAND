using System.Linq;
using PVZEngine;
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
        public static bool CanPlace(this LawnGrid grid, NamespaceID entityID)
        {
            return grid.CanPlace(grid.Level.Content.GetEntityDefinition(entityID));
        }
        public static bool CanPlace(this LawnGrid grid, EntityDefinition definition)
        {
            return grid.GetTakenEntities().Length <= 0;
        }
    }
}
