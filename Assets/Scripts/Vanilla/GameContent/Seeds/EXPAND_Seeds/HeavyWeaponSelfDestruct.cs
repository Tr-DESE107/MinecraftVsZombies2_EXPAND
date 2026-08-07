#nullable enable

using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.Vanilla.Audios;
using MVZ2Logic.Blueprints;
using MVZ2Logic.Definitions;
using MVZ2Logic.Entities;
using PVZEngine.Buffs;
using PVZEngine.Level;
using PVZEngine.SeedPacks;

namespace MVZ2.GameContent.Seeds
{
    [AutoSeedOptionDefinition(VanillaBlueprintNames.HeavyWeaponSelfDestruct)]
    public class HeavyWeaponSelfDestruct : SeedOptionDefinition
    {
        public HeavyWeaponSelfDestruct(string nsp, string name) : base(nsp, name) { }

        // 点一下即用型蓝图：SeedPack 与 LevelEngine 两个重载都转到统一的 Use(level)  
        public override void Use(SeedPack seedPack)
        {
            base.Use(seedPack);
            Use(seedPack.Level);
        }
        public override void Use(LevelEngine level, SeedDefinition seedDef)
        {
            base.Use(level, seedDef);
            Use(level);
        }

        // 可选：没有可自爆的器械时禁用蓝图（灰掉），避免空点浪费  
        public override void Update(SeedPack seedPack, float rechargeSpeed)
        {
            base.Update(seedPack, rechargeSpeed);
            bool valid = HeavyWeaponBlueprintUtils.FindRider(seedPack.Level) != null;
            seedPack.SetProperty(EngineSeedProps.DISABLE_ID, valid ? null : LogicBlueprintErrors.invalid);
        }

        private void Use(LevelEngine level)
        {
            // 通用性：索取矿车上的骑乘子实体，而不是写死超级狙击发射器  
            var rider = HeavyWeaponBlueprintUtils.FindRider(level);
            if (rider == null)
                return;

            // 已经在自爆倒计时中就不重复挂（避免刷新计时器）  
            if (rider.GetFirstBuff<HeavyWeaponSelfDestructBuff>() != null)
                return;

            // 挂上你写好的自爆 buff：进入无敌 → 倒计时 → 核爆 + 器械死亡  
            rider.AddBuff<HeavyWeaponSelfDestructBuff>();
            rider.PlaySound(VanillaSoundID.ironCurtain);   // 无敌启动音效，按需替换  
        }
    }
}
