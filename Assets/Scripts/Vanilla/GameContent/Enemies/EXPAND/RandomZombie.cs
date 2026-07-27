#nullable enable // 自动生成

using System.Linq;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Entities;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine.Buffs;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using Tools;
using UnityEngine;

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
            if (info.Effects.HasEffect(VanillaDamageEffects.REMOVE_ON_DEATH))
                return;
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

            //var grid = entity.GetGrid();
            //if (grid == null)
            //    return;

            var game = Global.Game;
            var level = entity.Level;
            var rng = entity.RNG;

            // 获取已解锁的敌人
            var unlockedEnemies = Global.Saves.GetUnlockedEnemies();

            // 筛选出有效的僵尸
            var validEnemies = unlockedEnemies.Where(id =>
            {
                // 如果提供了白名单，只选择白名单中的僵尸
                //if (whitelist != null && !whitelist.Contains(id))
                //    return false;

                // 检查是否在图鉴中
                if (!Global.Almanac.IsEnemyInAlmanac(id))
                    return false;

                // 检查当前关卡是否允许生成该敌人
                return true;
            });

            if (validEnemies.Count() <= 0)
                return;

            // 随机选一个僵尸
            var enemyID = validEnemies.Random(rng);

            // 在原位置生成僵尸
            var spawned = entity.SpawnWithParams(enemyID, entity.Position);

            entity.Remove();
        }
    }
}
