using System.Linq;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.Grids
{
    public static class VanillaGridExt
    {
        public static Entity GetCarrierEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.carrier);
        }
        public static Entity GetMainEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.main);
        }
        public static Entity GetProtectorEntity(this LawnGrid grid)
        {
            if (grid == null)
                return null;
            return grid.GetLayerEntity(VanillaGridLayers.protector);
        }
        public static bool CanPlaceEntity(this LawnGrid grid, NamespaceID entityID)
        {
            var status = grid.GetEntityPlaceStatus(entityID);
            return status == null;
        }
        public static NamespaceID GetEntityPlaceStatus(this LawnGrid grid, NamespaceID entityID)
        {
            var error = new TriggerResultNamespaceID();
            grid.Definition.CanPlaceEntity(grid, entityID, error);
            var level = grid.Level;
            foreach (var trigger in level.Triggers.GetTriggers(VanillaLevelCallbacks.CAN_PLACE_ENTITY))
            {
                if (!trigger.Filter(entityID))
                    continue;
                trigger.Run(grid, entityID, error);
            }
            return error.Result;
        }
        public static bool CanPlaceBlueprint(this LawnGrid grid, NamespaceID seedID, out NamespaceID error)
        {
            error = null;
            if (!NamespaceID.IsValid(seedID))
                return false;
            var level = grid.Level;
            var seedDef = level.Content.GetSeedDefinition(seedID);
            if (seedDef == null)
                return false;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var entityID = seedDef.GetSeedEntityID();
                error = GetEntityPlaceStatus(grid, entityID);
                return error == null;
            }
            return false;
        }
    }
}
