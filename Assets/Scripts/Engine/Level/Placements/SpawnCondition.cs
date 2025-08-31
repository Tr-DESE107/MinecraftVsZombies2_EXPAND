using PVZEngine.Entities;
using PVZEngine.Grids;

namespace PVZEngine.Placements
{
    public abstract class SpawnCondition
    {
        public abstract NamespaceID? GetSpawnError(PlacementDefinition placement, LawnGrid grid, EntityDefinition entity);
    }
}
