using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 公有方法
        public int GetSeedPackCount(bool ignoreEmpty = false)
        {
            if (ignoreEmpty)
                return seedPacks.Count(s => s != null);
            return seedPacks.Count;
        }
        public void SetSeedPackCount(int count)
        {
            var allSeedPacks = GetAllSeedPacks();
            seedPacks.Clear();
            for (int i = 0; i < count; i++)
            {
                SeedPack seedPack = i < allSeedPacks.Length ? allSeedPacks[i] : null;
                seedPacks.Add(seedPack);
            }
        }
        public int GetSeedPackIndex(SeedPack seed)
        {
            return seedPacks.IndexOf(seed);
        }
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
            return seedPacks.FirstOrDefault(r => r != null && r.Definition.GetID() == seedRef);
        }
        /// <summary>
        /// 将卡牌的重装载时间设置为初始。
        /// </summary>
        public void SetRechargeTimeToStart(SeedPack seedPack)
        {
            if (seedPack == null)
                return;
            seedPack.SetStartRecharge(true);
        }

        /// <summary>
        /// 将卡牌的重装载时间设置为使用后。
        /// </summary>
        public void SetRechargeTimeToUsed(SeedPack seedPack)
        {
            if (seedPack == null)
                return;
            seedPack.SetStartRecharge(false);
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
            SeedDefinition seedDefinition = ContentProvider.GetSeedDefinition(seedRef);
            if (seedDefinition == null)
                return null;
            var rechargeID = seedDefinition.GetRechargeID();
            var rechargeDef = ContentProvider.GetRechargeDefinition(rechargeID);
            var maxRecharge = rechargeDef.GetStartMaxRecharge();
            float time = maxRecharge * RechargeTimeMultiplier;
            float progress = CurrentFlag <= 0 ? 0 : time;

            return new SeedPack(this, seedDefinition);
        }

        #endregion

        #region 属性字段
        public float RechargeSpeed { get; set; } = 1;
        public float RechargeTimeMultiplier { get; set; } = 1;
        private List<SeedPack> seedPacks = new List<SeedPack>();
        #endregion
    }
}
