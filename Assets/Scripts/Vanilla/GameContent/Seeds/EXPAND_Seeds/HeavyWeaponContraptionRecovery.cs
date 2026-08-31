#nullable enable  
  
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using MVZ2Logic.Level;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using UnityEngine;
  
namespace MVZ2.GameContent.Seeds  
{  
    // 器械回收升级：提升“被吞噬者吃掉的器械变回蓝图掉落物”的概率等级。  
    // 等级存在关卡属性上（吞噬者重生后依然保留），满级后自动禁用。  
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponContraptionRecovery)]  
    public class HeavyWeaponContraptionRecovery : SeedOptionDefinition  
    {  
        public HeavyWeaponContraptionRecovery(string nsp, string name) : base(nsp, name) { }  
        public override void Use(SeedPack seedPack) { base.Use(seedPack); Use(seedPack.Level); }  
        public override void Use(LevelEngine level, SeedDefinition seedDef) { base.Use(level, seedDef); Use(level); }  
  
        // 已满级时禁用（灰置）  
        public override void Update(SeedPack seedPack, float rechargeSpeed)  
        {  
            base.Update(seedPack, rechargeSpeed);
            var level = seedPack.Level;
            bool valid = Devourer.GetRecoveryLevel(level) < Devourer.MAX_RECOVERY_LEVEL;
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);
        }  
  
        private void Use(LevelEngine level)  
        {  
            int lvl = Mathf.Min(Devourer.GetRecoveryLevel(level) + 1, Devourer.MAX_RECOVERY_LEVEL);
            Devourer.SetRecoveryLevel(level, lvl);
            level.PlaySound(VanillaSoundID.gunReload);
        }  
    }  
}
