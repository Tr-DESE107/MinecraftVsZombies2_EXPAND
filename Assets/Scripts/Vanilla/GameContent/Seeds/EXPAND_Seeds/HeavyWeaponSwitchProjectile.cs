#nullable enable

using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using MVZ2Logic.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponSwitchProjectile)]
    public class HeavyWeaponSwitchProjectile : SeedOptionDefinition
    {
        public HeavyWeaponSwitchProjectile(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            if (rider == null) return;
            var cur = rider.GetProjectileID();
            var next = (cur == VanillaProjectileID.arrow) ? VanillaProjectileID.SniperBullet : VanillaProjectileID.arrow;
            rider.SetProperty(VanillaEntityProps.PROJECTILE_ID, next);
            rider.PlaySound(VanillaSoundID.gunReload);
        }
    }
}
