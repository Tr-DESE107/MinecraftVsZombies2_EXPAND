using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;

namespace MVZ2.Vanilla
{
    public static class BuiltinGrid
    {
        public static bool CanPlace(this LawnGrid grid, NamespaceID entityID)
        {
            return grid.CanPlace(grid.Level.ContentProvider.GetEntityDefinition(entityID));
        }
        public static bool CanPlace(this LawnGrid grid, EntityDefinition definition)
        {
            return grid.GetTakenEntities().Length <= 0;
        }
    }
}
