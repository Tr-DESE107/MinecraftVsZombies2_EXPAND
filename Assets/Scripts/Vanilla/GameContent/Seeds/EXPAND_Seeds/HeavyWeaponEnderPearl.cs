#nullable enable  
  
using MVZ2.GameContent.Buffs.Contraptions;  
using MVZ2.GameContent.Effects;  
using MVZ2.GameContent.Projectiles;  
using MVZ2.Vanilla.Audios;  
using MVZ2Logic.Blueprints;  
using MVZ2Logic.Definitions;  
using PVZEngine.Buffs;  
using PVZEngine.Level;  
using PVZEngine.SeedPacks;  
  
namespace MVZ2.GameContent.Seeds  
{  
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponEnderPearl)]  
    public class HeavyWeaponEnderPearl : SeedOptionDefinition  
    {  
        public HeavyWeaponEnderPearl(string nsp, string name) : base(nsp, name) { }  
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }  
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }  
        private void Use(LevelEngine level)  
        {  
            // 用矿车本体作为投掷源与标识父实体（珍珠落地后传送的正是矿车）  
            var cart = HeavyWeaponBlueprintUtils.FindCart(level);  
            if (cart == null) return;  
  
            // 阵营跟随矿车上的器械（找不到时回退到左阵营）  
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);  
            int faction = rider != null ? rider.GetFaction() : level.Option.LeftFaction;  
  
            // 立刻从矿车处生成靶子标识，并挂上通用倒计时投掷 buff  
            var origin = cart.GetCenter();  
            var marker = level.Spawn(VanillaEffectID.aimTarget, origin, cart);  
            if (marker == null) return;  
            marker.SetParent(cart);  
  
            var buff = marker.AddBuff<HeavyWeaponThrowMarkerBuff>();  
            buff.SetProperty(HeavyWeaponThrowMarkerBuff.PROP_TIMEOUT, HeavyWeaponThrowMarkerBuff.COUNTDOWN_FRAMES);  
            buff.SetProperty(HeavyWeaponThrowMarkerBuff.PROP_FACTION, faction);  
            buff.SetProperty(HeavyWeaponThrowMarkerBuff.PROP_SOUND_ID, VanillaSoundID.bow);  
            buff.SetProperty(HeavyWeaponThrowMarkerBuff.PROP_PROJECTILE_ID, VanillaProjectileID.EnderPearl);  
        }  
    }  
}
