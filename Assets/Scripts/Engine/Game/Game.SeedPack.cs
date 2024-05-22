using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PVZEngine
{
    public partial class Game
    {
        #region 公有方法
        public void SetSeedPacks(NamespaceID[] seedIDs)
        {
            var seeds = seedIDs.Select(i => {
                var seedDef = GetSeedDefinition(i);
                var cost = seedDef.GetProperty<int>(SeedProperties.COST);
                var maxRecharge = seedDef.GetProperty<int>(SeedProperties.START_RECHARGE_TIME);
                return new SeedPack(i)
                {
                    Cost = cost,
                    Recharge = 0,
                    MaxRecharge = maxRecharge
                };
            }).ToArray();
            SetSeedPacks(seeds);
        }
        public void SetSeedPacks(SeedPack[] seeds)
        {
            seedPacks.Clear();
            seedPacks.AddRange(seeds);
        }
        public SeedPack[] GetAllSeedPacks()
        {
            return seedPacks.ToArray();
        }
        public SeedPack GetSeedPackAt(int index)
        {
            return seedPacks[index];
        }
        public SeedPack GetSeedPack(NamespaceID seedRef)
        {
            return seedPacks.FirstOrDefault(r => r.SeedReference == seedRef);
        }
        /// <summary>
        /// 将卡牌的重装载时间设置为初始。
        /// </summary>
        /// <param name="id">要重置的卡牌ID。</param>
        public void SetRechargeTimeToStart(SeedPack seedPack)
        {
            var seedDefinition = GetSeedDefinition(seedPack.SeedReference);
            float time = seedDefinition.GetProperty<int>(SeedProperties.START_RECHARGE_TIME) * RechargeTimeMultiplier;
            seedPack.MaxRecharge = time;
        }

        /// <summary>
        /// 将卡牌的重装载时间设置为使用后。
        /// </summary>
        /// <param name="id">要重置的卡牌ID。</param>
        public void SetRechargeTimeToUsed(SeedPack seedPack)
        {
            var seedDefinition = GetSeedDefinition(seedPack.SeedReference);
            float time = seedDefinition.GetProperty<int>(SeedProperties.RECHARGE_TIME) * RechargeTimeMultiplier;
            seedPack.MaxRecharge = time;
        }

        /// <summary>
        /// 重置所有卡牌重装填进度。
        /// </summary>
        public void ResetAllRechargeProgress()
        {
            foreach (var seedPack in seedPacks)
            {
                SetRechargeTimeToStart(seedPack);
            }
        }
        #endregion


        #region 私有方法
        private void UpdateSeedRecharges()
        {
            foreach (var recharge in seedPacks)
            {
                recharge.UpdateRecharge(RechargeSpeed);
            }
        }

        private SeedPack CreateSeedPack(NamespaceID seedRef)
        {
            SeedDefinition seedDefinition = GetSeedDefinition(seedRef);
            float time = seedDefinition.GetProperty<int>(SeedProperties.START_RECHARGE_TIME) * RechargeTimeMultiplier;
            float progress = CurrentFlag <= 0 ? 0 : time;

            return new SeedPack(seedRef)
            {
                MaxRecharge = time,
                Recharge = progress
            };
        }

        #endregion

        #region 属性字段
        public float RechargeSpeed { get; set; }
        public float RechargeTimeMultiplier { get; set; }
        private List<SeedPack> seedPacks = new List<SeedPack>();
        #endregion
    }
}
