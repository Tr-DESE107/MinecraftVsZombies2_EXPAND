using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 公有方法

        #region 种子包
        public void InsertSeedPackAt(int index, NamespaceID id)
        {
            if (id == null)
                return;
            if (index < 0 || index >= seedPacks.Length)
                return;
            if (seedPacks[index] != null)
                return;
            var seedPack = PopSeedPackFromPool(id);
            seedPacks[index] = seedPack;
            NotifySeedPackChange(index);
        }
        public bool RemoveSeedPackAt(int index)
        {
            if (index < 0 || index >= seedPacks.Length)
                return false;
            if (seedPacks[index] == null)
                return false;
            PushSeedPackToPool(seedPacks[index]);
            seedPacks[index] = null;
            NotifySeedPackChange(index);
            return true;
        }
        public void ReplaceSeedPackAt(int index, NamespaceID id)
        {
            if (index < 0 || index >= seedPacks.Length)
                return;
            RemoveSeedPackAt(index);
            InsertSeedPackAt(index, id);
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
            var targetCount = targetsID.Count();
            for (int i = 0; i < seedPacks.Length; i++)
            {
                var seed = i < targetCount ? targetsID.ElementAt(i) : null;
                ReplaceSeedPackAt(i, seed);
            }
        }
        public int GetSeedPackIndex(SeedPack seed)
        {
            return Array.IndexOf(seedPacks, seed);
        }
        public int GetSeedPackIndex(NamespaceID id)
        {
            return Array.FindIndex(seedPacks, s => s.GetDefinitionID() == id);
        }
        public int GetSeedPackCount()
        {
            return seedPacks.Count(s => s != null);
        }
        public SeedPack[] GetAllSeedPacks()
        {
            return seedPacks.ToArray();
        }
        public NamespaceID GetSeedPackIDAt(int index)
        {
            return seedPacks[index]?.GetDefinitionID();
        }
        public SeedPack GetSeedPackAt(int index)
        {
            return seedPacks[index];
        }
        public SeedPack GetSeedPack(NamespaceID seedRef)
        {
            return seedPacks.FirstOrDefault(r => r != null && r.GetDefinitionID() == seedRef);
        }
        #endregion

        #region 种子栏位
        public int GetSeedSlotCount()
        {
            return seedPacks.Length;
        }
        public void SetSeedSlotCount(int count)
        {
            var oldSeedPacks = seedPacks.ToArray();
            seedPacks = new SeedPack[count];
            OnSeedSlotCountChanged?.Invoke(count);

            ClearSeedPacks();
            ReplaceSeedPacks(oldSeedPacks.Select(s => s.GetDefinitionID()));
        }
        #endregion

        #region 种子池
        public SeedPack[] GetSeedPackPool()
        {
            return seedPackPool.ToArray();
        }
        #endregion

        #region 充能
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
            foreach (var seedPack in seedPackPool)
            {
                if (seedPack == null)
                    continue;
                seedPack.Update(RechargeSpeed);
            }
        }

        #region 种子池
        private SeedPack PopSeedPackFromPool(NamespaceID seedRef)
        {
            if (seedRef == null)
                return null;
            SeedDefinition seedDefinition = ContentProvider.GetSeedDefinition(seedRef);
            if (seedDefinition == null)
                return null;
            var seedPack = seedPackPool.FirstOrDefault(s => s.Definition.GetID() == seedRef);
            if (seedPack != null)
            {
                seedPackPool.Remove(seedPack);
                return seedPack;
            }
            else
            {
                seedPack = new SeedPack(this, seedDefinition);
                seedPack.SetStartRecharge(true);
                return seedPack;
            }
        }
        private void PushSeedPackToPool(SeedPack seed)
        {
            seedPackPool.Add(seed);
        }
        #endregion

        #endregion

        public event Action<int> OnSeedSlotCountChanged;
        public event Action<int> OnSeedPackChanged;
        #region 属性字段
        public float RechargeSpeed { get; set; } = 1;
        public float RechargeTimeMultiplier { get; set; } = 1;
        private SeedPack[] seedPacks = Array.Empty<SeedPack>();
        private List<SeedPack> seedPackPool = new List<SeedPack>();
        #endregion
    }
}
