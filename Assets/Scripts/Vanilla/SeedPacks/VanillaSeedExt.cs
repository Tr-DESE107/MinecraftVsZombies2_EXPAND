using MVZ2Logic;
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
