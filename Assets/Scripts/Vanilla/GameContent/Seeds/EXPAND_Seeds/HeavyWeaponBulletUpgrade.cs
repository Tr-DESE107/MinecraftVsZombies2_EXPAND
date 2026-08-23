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

        // 每帧刷新禁用状态：无器械 或 已升级 时禁用蓝图，避免升级后重复点击浪费  
        public override void Update(SeedPack seedPack, float rechargeSpeed)
        {
            base.Update(seedPack, rechargeSpeed);
            var rider = HeavyWeaponBlueprintUtils.FindRider(seedPack.Level);
            // 有器械且尚未升级子弹才可用  
            bool valid = rider != null && !rider.GetProperty<bool>(MegaSnipenser.PROP_BULLET_UPGRADED);
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);
        }

        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            if (rider == null) return;
            rider.SetProperty(MegaSnipenser.PROP_BULLET_UPGRADED, true);
            rider.PlaySound(VanillaSoundID.gunReload);
        }
    }
}
