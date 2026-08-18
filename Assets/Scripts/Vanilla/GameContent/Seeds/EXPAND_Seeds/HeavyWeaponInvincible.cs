#nullable enable

using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponInvincible)]
    public class HeavyWeaponInvincible : SeedOptionDefinition
    {
        public HeavyWeaponInvincible(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            if (rider == null) return;
            var buff = rider.AddBuff<IronCurtainBuff>();
            buff?.SetProperty(IronCurtainBuff.PROP_TIMEOUT, 300);
        }
    }
}
