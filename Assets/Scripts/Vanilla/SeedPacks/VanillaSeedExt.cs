using System.Linq;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;

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
                var stackOnEntity = entityDef.GetStackOnEntity();
                if (NamespaceID.IsValid(stackOnEntity))
                {
                    var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(stackOnEntity));
                    if (entity != null && entity.Exists() && entity.CanStackFrom(entityID))
                    {
                        entity.StackFromEntity(entityID);
                        PostUseEntityBlueprint(seed, entity, heldItemData);
                        return;
                    }
                }
                var upgradeFromEntity = entityDef.GetUpgradeFromEntity();
                if (NamespaceID.IsValid(upgradeFromEntity))
                {
                    var entity = grid.GetEntities().FirstOrDefault(e => e.IsEntityOf(upgradeFromEntity));
                    if (entity != null && entity.Exists())
                    {
                        var upgraded = entity.UpgradeToContraption(entityID);
                        PostUseEntityBlueprint(seed, upgraded, heldItemData);
                        return;
                    }
                }
                var placedEntity = grid.PlaceEntity(entityID);
                PostUseEntityBlueprint(seed, placedEntity, heldItemData);
            }
        }
        private static void PostUseEntityBlueprint(SeedPack seed, Entity entity, IHeldItemData heldItemData)
        {
            if (entity == null)
                return;
            if (heldItemData.InstantTrigger && entity.CanTrigger())
            {
                entity.Trigger();
            }
            var drawnFromPool = seed.GetDrawnConveyorSeed();
            if (NamespaceID.IsValid(drawnFromPool))
            {
                entity.AddTakenConveyorSeed(drawnFromPool);
            }
            seed.Level.Triggers.RunCallback(VanillaLevelCallbacks.POST_USE_ENTITY_BLUEPRINT, c => c(seed, entity));
        }
    }
}
