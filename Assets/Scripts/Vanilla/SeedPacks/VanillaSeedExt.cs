using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.HeldItems;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.SeedPacks;
using PVZEngine;
using PVZEngine.Definitions;
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
        public static bool CanInstantEvoke(this SeedPack seedPack)
        {
            var blueprintDef = seedPack?.Definition;
            return blueprintDef.CanInstantEvoke();
        }
    }
}
