﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.SeedPacks;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 创建种子包
        public ClassicSeedPack CreateSeedPack(NamespaceID id)
        {
            var def = Content.GetSeedDefinition(id);
            return new ClassicSeedPack(this, def, AllocSeedPackID());
        }
        public void InsertSeedPackAt(int index, ClassicSeedPack seed)
        {
            if (seed == null)
                return;
            if (index < 0 || index >= seedPacks.Length)
                return;
            if (seedPacks[index] != null)
                return;
            seedPacks[index] = seed;
            seed.PostAdd(this);
            OnSeedAdded?.Invoke(index);
        }
        private long AllocSeedPackID()
        {
            var id = currentSeedPackID;
            currentSeedPackID++;
            return id;
        }
        #endregion

        #region 移除种子包
        public bool RemoveSeedPackAt(int index)
        {
            if (index < 0 || index >= seedPacks.Length)
                return false;
            var seedPack = seedPacks[index];
            if (seedPack == null)
                return false;
            seedPacks[index] = null;
            seedPack.PostRemove(this);
            OnSeedRemoved?.Invoke(index);
            return true;
        }
        public void ClearSeedPacks()
        {
            for (int i = 0; i < GetSeedSlotCount(); i++)
            {
                RemoveSeedPackAt(i);
            }
        }
        #endregion

        #region 替换种子包
        public void ReplaceSeedPackAt(int index, ClassicSeedPack seed)
        {
            if (index < 0 || index >= GetSeedSlotCount())
                return;
            RemoveSeedPackAt(index);
            InsertSeedPackAt(index, seed);
        }
        public void ReplaceSeedPacks(IEnumerable<ClassicSeedPack> targetsID)
        {
            var targetCount = targetsID.Count();
            for (int i = 0; i < GetSeedSlotCount(); i++)
            {
                var seed = i < targetCount ? targetsID.ElementAt(i) : null;
                ReplaceSeedPackAt(i, seed);
            }
        }
        #endregion

        #region 获取种子包位置
        public int GetSeedPackIndex(ClassicSeedPack seed)
        {
            return Array.IndexOf(seedPacks, seed);
        }
        public int GetSeedPackIndex(NamespaceID id)
        {
            return Array.FindIndex(seedPacks, s => s.GetDefinitionID() == id);
        }
        #endregion

        #region 获取种子包数量
        public int GetSeedPackCount()
        {
            return seedPacks.Count(s => s != null);
        }
        public int GetSeedSlotCount()
        {
            return seedPacks.Length;
        }
        public void SetSeedSlotCount(int count)
        {
            if (count < seedPacks.Length)
            {
                for (int i = count; i < seedPacks.Length; i++)
                {
                    RemoveSeedPackAt(i);
                }
            }
            Array.Resize(ref seedPacks, count);
            OnSeedSlotCountChanged?.Invoke(count);
        }
        #endregion

        #region 获取种子包
        public ClassicSeedPack?[] GetAllSeedPacks()
        {
            return seedPacks.ToArray();
        }
        public ClassicSeedPack? GetSeedPackAt(int index)
        {
            if (index < 0 || index >= seedPacks.Length)
                return null;
            return seedPacks[index];
        }
        public ClassicSeedPack? GetSeedPack(NamespaceID seedRef)
        {
            return seedPacks.FirstOrDefault(r => r != null && r.GetDefinitionID() == seedRef);
        }
        public ClassicSeedPack? GetSeedPackByID(long id)
        {
            return seedPacks.FirstOrDefault(s => s != null && s.ID == id);
        }
        #endregion

        #region 更新
        private void UpdateClassicSeedPacks(float rechargeSpeed)
        {
            foreach (var seedPack in seedPacks)
            {
                if (seedPack == null)
                    continue;
                seedPack.Update(rechargeSpeed);
            }
        }
        #endregion

        #region 充能
        /// <summary>
        /// 重置所有卡牌重装填进度。
        /// </summary>
        public void ResetAllRechargeProgress()
        {
            foreach (var seedPack in seedPacks)
            {
                if (seedPack == null)
                    continue;
                seedPack.SetStartRecharge(true);
                seedPack.ResetRecharge();
            }
        }
        public void FullRechargeAll()
        {
            foreach (var seedPack in seedPacks)
            {
                if (seedPack == null)
                    continue;
                seedPack.FullRecharge();
            }
        }
        #endregion

        #region 序列化
        private void WriteSeedPacksToSerializable(SerializableLevel seri)
        {
            seri.seedPacks = seedPacks.Select(g => g != null ? g.Serialize() : null).ToArray();
        }
        private void CreateSeedPacksFromSerializable(SerializableLevel seri)
        {
            seedPacks = seri.seedPacks.Select(g => g != null ? ClassicSeedPack.Deserialize(g, this) : null).ToArray();
        }
        private void ReadSeedPacksFromSerializable(SerializableLevel seri)
        {
            foreach (var seed in seedPacks)
            {
                if (seed == null)
                    continue;
                var seriSeed = seri.seedPacks.FirstOrDefault(s => s.id == seed.ID);
                if (seriSeed == null)
                    continue;
                seed.ApplyDeserializedProperties(this, seriSeed);
            }
        }
        #endregion

        public event Action<int> OnSeedAdded;
        public event Action<int> OnSeedRemoved;
        public event Action<int> OnSeedSlotCountChanged;

        private long currentSeedPackID = 1;
        private ClassicSeedPack[] seedPacks = Array.Empty<ClassicSeedPack>();
    }
}
