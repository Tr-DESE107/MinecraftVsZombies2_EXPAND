#nullable enable

using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using MVZ2Logic.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponBulletUpgrade)]
    public class HeavyWeaponBulletUpgrade : SeedOptionDefinition
    {
        public HeavyWeaponBulletUpgrade(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            if (rider == null) return;
            rider.SetProperty(MegaSnipenser.PROP_BULLET_UPGRADED, true);
            rider.PlaySound(VanillaSoundID.gunReload);
        }
    }
}
