#nullable enable  
  
using MVZ2.GameContent.Buffs.Contraptions;   // HeavyWeaponAttackUpBuff  
using MVZ2.Vanilla.Audios;  
using MVZ2Logic.Blueprints;  
using MVZ2Logic.Definitions;  
using MVZ2Logic.Entities;                     // GetFirstBuff / AddBuff / PlaySound  
using PVZEngine.Buffs;  
using PVZEngine.Level;  
using PVZEngine.SeedPacks;  
  
namespace MVZ2.GameContent.Seeds  
{  
    // 攻击力增加：对矿车上的骑乘器械叠加伤害升级 buff，满级后自动禁用。  
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponAttackUp)]  
    public class HeavyWeaponAttackUp : SeedOptionDefinition  
    {  
        public HeavyWeaponAttackUp(string nsp, string name) : base(nsp, name) { }  
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }  
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }  
  
        // 每帧刷新禁用状态：无器械 或 已满级 时禁用蓝图，避免浪费  
        public override void Update(SeedPack seedPack, float rechargeSpeed)  
        {  
            base.Update(seedPack, rechargeSpeed);  
            var rider = HeavyWeaponBlueprintUtils.FindRider(seedPack.Level);  
            var buff = rider?.GetFirstBuff<HeavyWeaponAttackUpBuff>();  
            int level = buff != null ? HeavyWeaponAttackUpBuff.GetLevel(buff) : 0;  
            bool valid = rider != null && level < HeavyWeaponAttackUpBuff.MAX_LEVEL;  
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);  
        }  
  
        private void Use(LevelEngine level)  
        {  
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);  
            if (rider == null) return;  
            // 已有 buff 就复用，否则新挂一个，再升一级  
            var buff = rider.GetFirstBuff<HeavyWeaponAttackUpBuff>() ?? rider.AddBuff<HeavyWeaponAttackUpBuff>();  
            HeavyWeaponAttackUpBuff.Upgrade(buff);  
            rider.PlaySound(VanillaSoundID.gunReload);  
        }  
    }  
}
