using System.Linq;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;
using UnityEngine;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedExt
    {
        public static bool CanInstantTrigger(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.IsTriggerActive() && blueprintDef.CanInstantTrigger();
        }
        public static void UseOnGrid(this SeedPack seed, LawnGrid grid, IHeldItemData heldItemData)
        {
            var seedDef = seed.Definition;
            if (seedDef.GetSeedType() == SeedTypes.ENTITY)
            {
                var level = seed.Level;
                var entityID = seedDef.GetSeedEntityID();
                var entityDef = level.Content.GetEntityDefinition(entityID);
                if (entityDef == null)
                    return;
                var upgradeFromEntity = entityDef.GetUpgradeFromEntity();
                if (NamespaceID.IsValid(upgradeFromEntity))
                {
                    var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(upgradeFromEntity));
                    if (entity != null && entity.Exists())
                    {
                        seed.UpgradeToContraption(entity, entityID);
                        return;
                    }
                }
                var stackOnEntity = entityDef.GetStackOnEntity();
                if (NamespaceID.IsValid(stackOnEntity))
                {
                    var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(stackOnEntity));
                    if (entity != null && entity.Exists() && entity.CanStackFrom(entityID))
                    {
                        seed.StackEntityOnGrid(entity, entityID);
                        return;
                    }
                }
                seed.PlaceEntityOnGrid(grid, entityID, heldItemData);
            }
        }
        public static void StackEntityOnGrid(this SeedPack seed, Entity target, NamespaceID entityID)
        {
            target.StackFromEntity(entityID);
            var drawnFromPool = seed.GetDrawnConveyorSeed();
            if (NamespaceID.IsValid(drawnFromPool))
            {
                target.AddTakenConveyorSeed(drawnFromPool);
            }
        }
        public static void UpgradeToContraption(this SeedPack seed, Entity target, NamespaceID entityID)
        {
            target.UpgradeToContraption(entityID);
            var drawnFromPool = seed.GetDrawnConveyorSeed();
            if (NamespaceID.IsValid(drawnFromPool))
            {
                target.AddTakenConveyorSeed(drawnFromPool);
            }
        }
        public static void PlaceEntityOnGrid(this SeedPack seed, LawnGrid grid, NamespaceID entityID, IHeldItemData heldItemData)
        {
            var entity = grid.PlaceEntity(entityID);
            if (entity != null)
            {
                var drawnFromPool = seed.GetDrawnConveyorSeed();
                if (NamespaceID.IsValid(drawnFromPool))
                {
                    entity.AddTakenConveyorSeed(drawnFromPool);
                }
                if (heldItemData.InstantTrigger && entity.CanTrigger())
                {
                    entity.Trigger();
                }
            }
        }
    }
}
