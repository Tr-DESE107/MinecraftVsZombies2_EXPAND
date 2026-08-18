#nullable enable

using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.heavyWeaponFlashbang)]
    public class HeavyWeaponFlashbang : SeedOptionDefinition
    {
        public HeavyWeaponFlashbang(string nsp, string name) : base(nsp, name) { }
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }
        private void Use(LevelEngine level)
        {
            var cart = HeavyWeaponBlueprintUtils.FindRider(level);
            if (cart == null) return;

            // 阵营跟随矿车上的器械（找不到骑乘者时回退到左阵营）  
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            int faction = rider != null ? rider.GetFaction() : level.Option.LeftFaction;

            // 立刻从矿车处生成靶子标识，并挂上倒计时 buff  
            var origin = cart.GetCenter();
            var marker = level.Spawn(VanillaEffectID.aimTarget, origin, cart);
            if (marker == null) return;
            marker.SetParent(cart);  // 记录矿车引用，供 buff 抛投时取源点  

            var buff = marker.AddBuff<HeavyWeaponFlashbangMarkerBuff>();
            buff.SetProperty(HeavyWeaponFlashbangMarkerBuff.PROP_TIMEOUT, HeavyWeaponFlashbangMarkerBuff.COUNTDOWN_FRAMES);
            buff.SetProperty(HeavyWeaponFlashbangMarkerBuff.PROP_FACTION, faction);
        }
    }
}
