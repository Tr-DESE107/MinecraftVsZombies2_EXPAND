using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Grids;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using PVZEngine.Triggers;

namespace MVZ2.Vanilla.SeedPacks
{
    public static class VanillaSeedExt
    {
        public static bool CanPick(this SeedPack seed)
        {
            return seed.CanPick(out _);
        }
        public static bool CanPick(this SeedPack seed, out string errorMessage)
        {
            if (seed is ClassicSeedPack)
            {
                if (seed == null)
                {
                    errorMessage = null;
                    return false;
                }
                if (!seed.IsCharged())
                {
                    errorMessage = Global.Game.GetText(VanillaStrings.TOOLTIP_RECHARGING);
                    return false;
                }
                if (seed.Level.Energy < seed.GetCost())
                {
                    errorMessage = Global.Game.GetText(Vanilla.VanillaStrings.TOOLTIP_NOT_ENOUGH_ENERGY);
                    return false;
                }
                if (seed.IsDisabled())
                {
                    errorMessage = Global.Game.GetText(seed.GetDisableMessage());
                    return false;
                }
            }
            errorMessage = null;
            return true;
        }
        public static bool CanInstantTrigger(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.IsTriggerActive() && blueprintDef.CanInstantTrigger();
        }
        public static bool CanImbue(this SeedPack seedPack)
        {
            if (seedPack.IsImbued())
            {
                return false;
            }
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.CanImbue();
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
            if (heldItemData.Imbued && entity.CanEvoke())
            {
                seed.SetImbued(false);
                entity.Evoke();
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
