#nullable enable

using MVZ2.Vanilla.Entities;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponHeal)]
    public class HeavyWeaponHeal : SeedOptionDefinition
    {
        public HeavyWeaponHeal(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            rider?.HealEffects(rider.GetMaxHealth() * 0.3f, rider);
        }
        public const float THROW_DISTANCE = 400f;
        public const float THROW_ARC = 200f;
    }
}
