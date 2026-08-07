#nullable enable

using MVZ2.Vanilla.Entities;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponRegen)]
    public class HeavyWeaponRegen : SeedOptionDefinition
    {
        public HeavyWeaponRegen(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            rider?.InflictRegenerationBuff(2f, 600, null);   // 每帧+2，持续10秒  
        }
    }
}
