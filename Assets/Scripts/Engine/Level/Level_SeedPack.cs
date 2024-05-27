using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PVZEngine
{
    public partial class Level
    {
        #region 公有方法
        public void SetSeedPacks(NamespaceID[] seedIDs)
        {
            var seeds = seedIDs.Select(i => CreateSeedPack(i)).ToArray();
            SetSeedPacks(seeds);
        }
        public void SetSeedPacks(SeedPack[] seeds)
        {
            seedPacks.Clear();
            seedPacks.AddRange(seeds);
        }
        public SeedPack[] GetAllSeedPacks(bool ignoreEmpty = false)
        {
            if (ignoreEmpty)
                return seedPacks.Where(s => s != null).ToArray();
            return seedPacks.ToArray();
        }
        public SeedPack GetSeedPackAt(int index)
        {
            return seedPacks[index];
        }
        public SeedPack GetSeedPack(NamespaceID seedRef)
        {
            return seedPacks.FirstOrDefault(r => r != null && r.SeedReference == seedRef);
        }
        /// <summary>
        /// 将卡牌的重装载时间设置为初始。
        /// </summary>
        /// <param name="id">要重置的卡牌ID。</param>
        public void SetRechargeTimeToStart(SeedPack seedPack)
        {
            if (seedPack == null)
                return;
            var seedDefinition = GetSeedDefinition(seedPack.SeedReference);
            var rechargeID = seedDefinition.GetRechargeID();
            var rechargeDef = GetRechargeDefinition(rechargeID);
            var maxRecharge = rechargeDef.GetStartMaxRecharge();
            float time = maxRecharge * RechargeTimeMultiplier;
            seedPack.MaxRecharge = time;
        }

        /// <summary>
        /// 将卡牌的重装载时间设置为使用后。
        /// </summary>
        /// <param name="id">要重置的卡牌ID。</param>
        public void SetRechargeTimeToUsed(SeedPack seedPack)
        {
            if (seedPack == null)
                return;
            var seedDefinition = GetSeedDefinition(seedPack.SeedReference);
            var rechargeID = seedDefinition.GetRechargeID();
            var rechargeDef = GetRechargeDefinition(rechargeID);
            var maxRecharge = rechargeDef.GetMaxRecharge();
            float time = maxRecharge * RechargeTimeMultiplier;
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
            foreach (var seedPack in seedPacks)
            {
                if (seedPack == null)
                    continue;
                seedPack.UpdateRecharge(RechargeSpeed);
            }
        }

        private SeedPack CreateSeedPack(NamespaceID seedRef)
        {
            if (seedRef == null)
                return null;
            SeedDefinition seedDefinition = GetSeedDefinition(seedRef);
            if (seedDefinition == null)
                return null;
            var rechargeID = seedDefinition.GetRechargeID();
            var rechargeDef = GetRechargeDefinition(rechargeID);
            var maxRecharge = rechargeDef.GetStartMaxRecharge();
            float time = maxRecharge * RechargeTimeMultiplier;
            float progress = CurrentFlag <= 0 ? 0 : time;

            return new SeedPack(seedRef)
            {
                Cost = seedDefinition.GetCost(),
                MaxRecharge = time,
                Recharge = progress
            };
        }

        #endregion

        #region 属性字段
        public float RechargeSpeed { get; set; } = 1;
        public float RechargeTimeMultiplier { get; set; } = 1;
        private List<SeedPack> seedPacks = new List<SeedPack>();
        #endregion
    }
}
