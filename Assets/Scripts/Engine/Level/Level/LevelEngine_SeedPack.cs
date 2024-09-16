using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
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
            return seedPacks.Length;
        }
        public void SetSeedPackCount(int count)
        {
            var oldSeedPacks = seedPacks.ToArray();
            seedPacks = new SeedPack[count];
            OnSeedPackCountChanged?.Invoke(count);
            ReplaceSeedPacks(oldSeedPacks);
        }
        public void ClearSeedPacks()
        {
            for (int i = 0; i < seedPacks.Length; i++)
            {
                RemoveSeedPackAt(i);
            }
        }
        public void ReplaceSeedPacks(IEnumerable<NamespaceID> targetsID)
        {
            ReplaceSeedPacks(targetsID.Select(t => CreateSeedPack(t)));
        }
        public void ReplaceSeedPacks(IEnumerable<SeedPack> targetPacks)
        {
            var targetCount = targetPacks.Count();
            for (int i = 0; i < seedPacks.Length; i++)
            {
                var seed = i < targetCount ? targetPacks.ElementAt(i) : null;
                SetSeedPackAt(i, seed);
            }
        }
        public int GetSeedPackIndex(SeedPack seed)
        {
            return Array.IndexOf(seedPacks, seed);
        }
        public SeedPack[] GetAllSeedPacks()
        {
            return seedPacks.ToArray();
        }
        public SeedPack GetSeedPackAt(int index)
        {
            return seedPacks[index];
        }
        public void SetSeedPackAt(int index, NamespaceID seedID)
        {
            var seed = CreateSeedPack(seedID);
            SetSeedPackAt(index, seedID);
        }
        public void SetSeedPackAt(int index, SeedPack seed)
        {
            if (index < 0 || index >= seedPacks.Length)
                return;
            seedPacks[index] = seed;
            NotifySeedPackChange(index);
        }
        public void RemoveSeedPackAt(int index)
        {
            if (index < 0 || index >= seedPacks.Length)
                return;
            seedPacks[index] = null;
            NotifySeedPackChange(index);
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
        private void NotifySeedPackChange(int index)
        {
            OnSeedPackChanged?.Invoke(index);
        }
        private void UpdateSeedPacks()
        {
            foreach (var seedPack in seedPacks)
            {
                if (seedPack == null)
                    continue;
                seedPack.Update(RechargeSpeed);
            }
        }

        private SeedPack CreateSeedPack(NamespaceID seedRef)
        {
            if (seedRef == null)
                return null;
            SeedDefinition seedDefinition = ContentProvider.GetSeedDefinition(seedRef);
            if (seedDefinition == null)
                return null;
            var seedPack = new SeedPack(this, seedDefinition);
            seedPack.SetStartRecharge(true);
            return seedPack;
        }

        #endregion

        public event Action<int> OnSeedPackCountChanged;
        public event Action<int> OnSeedPackChanged;
        #region 属性字段
        public float RechargeSpeed { get; set; } = 1;
        public float RechargeTimeMultiplier { get; set; } = 1;
        private SeedPack[] seedPacks = Array.Empty<SeedPack>();
        #endregion
    }
}
