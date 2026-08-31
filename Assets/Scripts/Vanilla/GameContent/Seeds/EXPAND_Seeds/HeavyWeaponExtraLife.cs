#nullable enable

using MVZ2.Vanilla.Audios;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponExtraLife)]
    public class HeavyWeaponExtraLife : SeedOptionDefinition
    {
        public HeavyWeaponExtraLife(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        public override void Update(SeedPack seedPack, float rechargeSpeed)
        {
            base.Update(seedPack, rechargeSpeed);
            bool valid = seedPack.Level.GetStarshardCount() < seedPack.Level.GetStarshardSlotCount();
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);
        }
        private void Use(LevelEngine level)
        {
            if (level.GetStarshardCount() < level.GetStarshardSlotCount())
                level.AddStarshardCount(1);
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            rider?.PlaySound(VanillaSoundID.gem);
        }
    }
}
