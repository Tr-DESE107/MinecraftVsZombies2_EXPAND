﻿using MVZ2.GameContent.Seeds;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

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
            var id = seed.GetPickError();
            if (NamespaceID.IsValid(id))
            {
                errorMessage = Global.Game.GetBlueprintErrorMessage(id);
                return false;
            }
            errorMessage = null;
            return true;
        }
        public static NamespaceID GetPickError(this SeedPack seed)
        {
            if (seed is ClassicSeedPack)
            {
                if (seed == null)
                {
                    return VanillaBlueprintErrors.invalid;
                }
                if (!seed.IsCharged())
                {
                    return VanillaBlueprintErrors.recharging;
                }
                if (seed.Level.Energy < seed.GetCost())
                {
                    return VanillaBlueprintErrors.notEnoughEnergy;
                }
                if (seed.IsDisabled())
                {
                    return seed.GetDisableID();
                }
            }
            return null;
        }
        public static NamespaceID GetSeedEntityID(this SeedPack seed)
        {
            var seedDef = seed?.Definition;
            if (seedDef == null)
                return null;
            if (seedDef.GetSeedType() != SeedTypes.ENTITY)
                return null;
            return seedDef.GetSeedEntityID();
        }
        public static bool CanInstantTrigger(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.IsTriggerActive() && blueprintDef.CanInstantTrigger();
        }
        public static bool CanInstantEvoke(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.CanInstantEvoke();
        }
    }
}
