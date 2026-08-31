#nullable enable  
  
using MVZ2.GameContent.Effects;   // MinecartRideable  
using MVZ2.Vanilla.Audios;  
using MVZ2Logic.Blueprints;  
using MVZ2Logic.Definitions;  
using MVZ2Logic.Entities;         // PlaySound  
using PVZEngine.Level;  
using PVZEngine.SeedPacks;  
using UnityEngine;                // Mathf  
  
namespace MVZ2.GameContent.Seeds  
{  
    // 速度增加：提升矿车移速等级（存在关卡属性上，跨矿车重生保留），满级后自动禁用。  
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponSpeedUp)]  
    public class HeavyWeaponSpeedUp : SeedOptionDefinition  
    {  
        public HeavyWeaponSpeedUp(string nsp, string name) : base(nsp, name) { }  
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }  
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }  
  
        // 无矿车 或 速度已满级 时禁用  
        public override void Update(SeedPack seedPack, float rechargeSpeed)  
        {  
            base.Update(seedPack, rechargeSpeed);  
            var level = seedPack.Level;  
            bool valid = HeavyWeaponBlueprintUtils.FindCart(level) != null  
                         && MinecartRideable.GetSpeedLevel(level) < MinecartRideable.MAX_SPEED_LEVEL;  
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);  
        }  
  
        private void Use(LevelEngine level)  
        {  
            var cart = HeavyWeaponBlueprintUtils.FindCart(level);  
            if (cart == null) return;  
            int lvl = Mathf.Min(MinecartRideable.GetSpeedLevel(level) + 1, MinecartRideable.MAX_SPEED_LEVEL);  
            MinecartRideable.SetSpeedLevel(level, lvl);  
            cart.PlaySound(VanillaSoundID.gunReload);  
        }  
    }  
}
