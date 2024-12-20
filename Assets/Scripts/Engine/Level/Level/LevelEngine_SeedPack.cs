using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Definitions;
using PVZEngine.SeedPacks;

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
        public int GetSeedPackIndex(ClassicSeedPack seed)
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
        public ClassicSeedPack[] GetAllSeedPacks()
        {
            return seedPacks.ToArray();
        }
        public NamespaceID GetSeedPackIDAt(int index)
        {
            return seedPacks[index]?.GetDefinitionID();
        }
        public ClassicSeedPack GetSeedPackAt(int index)
        {
            return seedPacks[index];
        }
        public ClassicSeedPack GetSeedPack(NamespaceID seedRef)
        {
            return seedPacks.FirstOrDefault(r => r != null && r.GetDefinitionID() == seedRef);
        }
        public ClassicSeedPack GetSeedPackByID(long id)
        {
            return seedPacks.FirstOrDefault(s => s.ID == id) ?? seedPackPool.FirstOrDefault(s => s.ID == id);
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
            seedPacks = new ClassicSeedPack[count];
            OnSeedSlotCountChanged?.Invoke(count);

            ClearSeedPacks();
            ReplaceSeedPacks(oldSeedPacks.Select(s => s?.GetDefinitionID()));
        }
        #endregion

        #region 传送带
        public bool CanConveySeedPack()
        {
            return conveyorSeedPacks.Count < GetConveyorSlotCount();
        }
        public ConveyorSeedPack AddConveyorSeedPack(NamespaceID id)
        {
            return InsertConveyorSeedPackAt(conveyorSeedPacks.Count, id);
        }
        public ConveyorSeedPack InsertConveyorSeedPackAt(int index, NamespaceID id)
        {
            if (id == null)
                return null;
            if (index < 0 || index > conveyorSeedPacks.Count || index >= GetConveyorSlotCount())
                return null;
            SeedDefinition seedDefinition = Content.GetSeedDefinition(id);
            if (seedDefinition == null)
                return null;
            var seedPack = new ConveyorSeedPack(this, seedDefinition, currentSeedPackID++);
            conveyorSeedPacks.Insert(index, seedPack);
            OnConveyorSeedAdded?.Invoke(index);
            return seedPack;
        }
        public bool RemoveConveyorSeedPackAt(int index)
        {
            if (index < 0 || index >= conveyorSeedPacks.Count)
                return false;
            var seedPack = conveyorSeedPacks[index];
            conveyorSeedPacks.RemoveAt(index);
            OnConveyorSeedRemoved?.Invoke(index);
            return true;
        }
        public void ClearConveyorSeedPacks()
        {
            for (int i = conveyorSeedPacks.Count - 1; i >= 0; i--)
            {
                RemoveSeedPackAt(i);
            }
        }
        public int GetConveyorSeedPackIndex(ConveyorSeedPack seed)
        {
            return conveyorSeedPacks.IndexOf(seed);
        }
        public int GetConveyorSeedPackIndex(NamespaceID id)
        {
            return conveyorSeedPacks.FindIndex(s => s.GetDefinitionID() == id);
        }
        public int GetConveyorSeedPackCount()
        {
            return conveyorSeedPacks.Count(s => s != null);
        }
        public ConveyorSeedPack[] GetAllConveyorSeedPacks()
        {
            return conveyorSeedPacks.ToArray();
        }
        public NamespaceID GetConveyorSeedPackIDAt(int index)
        {
            return conveyorSeedPacks[index]?.GetDefinitionID();
        }
        public ConveyorSeedPack GetConveyorSeedPackAt(int index)
        {
            return conveyorSeedPacks[index];
        }
        public ConveyorSeedPack GetConveyorSeedPack(NamespaceID seedRef)
        {
            return conveyorSeedPacks.FirstOrDefault(r => r != null && r.GetDefinitionID() == seedRef);
        }
        public ConveyorSeedPack GetConveyorSeedPackByID(long id)
        {
            return conveyorSeedPacks.FirstOrDefault(s => s.ID == id);
        }
        public int GetConveyorSlotCount()
        {
            return conveyorSlotCount;
        }
        public void SetConveyorSlotCount(int value)
        {
            conveyorSlotCount = value;
            OnConveyorSeedSlotCountChanged?.Invoke(value);
        }
        public void PutSeedToConveyorPool(NamespaceID seedID)
        {
            conveyorSeedSpendRecord.AddSpendValue(seedID, -1);
        }
        public void SpendSeedFromConveyorPool(NamespaceID seedID)
        {
            conveyorSeedSpendRecord.AddSpendValue(seedID, 1);
        }
        public int GetSpentSeedFromConveyorPool(NamespaceID seedID)
        {
            return conveyorSeedSpendRecord.GetSpendValue(seedID);
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
        private ClassicSeedPack PopSeedPackFromPool(NamespaceID seedRef)
        {
            if (seedRef == null)
                return null;
            SeedDefinition seedDefinition = Content.GetSeedDefinition(seedRef);
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
                seedPack = new ClassicSeedPack(this, seedDefinition, currentSeedPackID++);
                seedPack.SetStartRecharge(true);
                return seedPack;
            }
        }
        private void PushSeedPackToPool(ClassicSeedPack seed)
        {
            seedPackPool.Add(seed);
        }
        #endregion

        #endregion

        public event Action<int> OnSeedSlotCountChanged;
        public event Action<int> OnSeedPackChanged;
        public event Action<int> OnConveyorSeedAdded;
        public event Action<int> OnConveyorSeedRemoved;
        public event Action<int> OnConveyorSeedSlotCountChanged;
        #region 属性字段
        public float RechargeSpeed { get; set; } = 1;
        public float RechargeTimeMultiplier { get; set; } = 1;
        private int conveyorSlotCount = 10;
        private long currentSeedPackID = 1;
        private ClassicSeedPack[] seedPacks = Array.Empty<ClassicSeedPack>();
        private List<ClassicSeedPack> seedPackPool = new List<ClassicSeedPack>();
        private List<ConveyorSeedPack> conveyorSeedPacks = new List<ConveyorSeedPack>();
        private ConveyorSeedSpendRecords conveyorSeedSpendRecord = new ConveyorSeedSpendRecords();
        #endregion
    }
}
