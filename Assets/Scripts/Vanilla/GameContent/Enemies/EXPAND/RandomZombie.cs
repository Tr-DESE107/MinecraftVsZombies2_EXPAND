#nullable enable // 自动生成

using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Entities;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Entities;
using UnityEngine;
using MVZ2.GameContent.Models;
using PVZEngine.Definitions;
using PVZEngine;

namespace MVZ2.GameContent.Enemies
{
    [AutoEntityBehaviourDefinition(VanillaEnemyNames.RandomZombie)]
    public class RandomZombie : AIEntityBehaviour
    {
        public RandomZombie(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            var level = entity.Level;
            var lane = entity.GetLane();
            // 如果是水车道或空车道，添加船只Buff
            if (level.IsWaterLane(lane) || level.IsAirLane(lane))
            {
                entity.AddBuff<BoatBuff>();
                entity.SetModelProperty("HasBoat", true);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // 生成破碎船只效果
                entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity)?.Let(effect =>
                {
                    // 设置特效的随机速度
                    effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                    // 更换为船只物品模型
                    effect.ChangeModel(VanillaModelID.boatItem);
                    // 保持缩放比例
                    effect.SetDisplayScale(entity.GetDisplayScale());
                });
            }

            // 死亡后召唤僵尸种族（Zombie_Exp，使用 <zombie> 模板）的敌人
            HealthLossSpawnHelper.SpawnOnDeath(entity, info, SpawnWhitelist, SpawnWeights);
        }

        #region 可生成敌人加权池

        // 僵尸种族（mvz2:Zombie_Exp）敌人列表，不含随机系列与彩蛋
        private static NamespaceID[] SpawnWhitelist = new NamespaceID[]
        {
            VanillaEnemyID.zombie,
            VanillaEnemyID.flagZombie,
            VanillaEnemyID.leatherCappedZombie,
            VanillaEnemyID.ironHelmettedZombie,
            VanillaEnemyID.diamondHelmettedZombie,
            VanillaEnemyID.reflectiveBarrierZombie,
            VanillaEnemyID.talismanZombie,
            VanillaEnemyID.wickedHermitZombie,
            VanillaEnemyID.shikaisenZombie,
            VanillaEnemyID.emperorZombie,
            VanillaEnemyID.cannoneerZombie,
            VanillaEnemyID.MegaZombie,
            VanillaEnemyID.SuperMegaZombie,
            VanillaEnemyID.HostZombie,
            VanillaEnemyID.BloodlustHostZombie,
            VanillaEnemyID.EnragedHostZombie,
            VanillaEnemyID.MonkZombie,
            VanillaEnemyID.LeatherMonkZombie,
            VanillaEnemyID.IronMonkZombie,
            VanillaEnemyID.FlagbearerZombie,
            VanillaEnemyID.SixQiZombie,
            VanillaEnemyID.TorchKongfuZombie,
            VanillaEnemyID.DrunkardMonkZombie,
            VanillaEnemyID.Hemperor,
            VanillaEnemyID.PirateZombie,
            VanillaEnemyID.LeatherPirateZombie,
            VanillaEnemyID.KogasaZombie,
            VanillaEnemyID.ChiefCannoneerZombie,
            VanillaEnemyID.MusketeerZombie,
            VanillaEnemyID.JollyRogerZombie,
            VanillaEnemyID.SailorZombie,
        };

        // 对应每种敌人的生成权重，数值越大越容易被抽中（默认 1，待细调）
        private static int[] SpawnWeights = new int[]
        {
            1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,
            1, 1, 1, 1, 1,
        };

        #endregion
    }
}
