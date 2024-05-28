using PVZEngine;

namespace MVZ2.Vanilla
{
    public static class MVZ2Grid
    {
        public static bool CanPlace(this LawnGrid grid, NamespaceID entityID)
        {
            return grid.CanPlace(grid.Level.GetEntityDefinition(entityID));
        }
        public static bool CanPlace(this LawnGrid grid, EntityDefinition definition)
        {
            return grid.GetTakenEntities().Length <= 0;
        }
    }
}
