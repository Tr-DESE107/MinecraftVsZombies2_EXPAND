//#define USE_ALTERNATE

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.SeedPacks
{
    public abstract class SeedPool
    {
        #region 属性

        public int TypeCount => cardMaxCounts.Count;
        protected Dictionary<NamespaceID, int> cardMaxCounts = new Dictionary<NamespaceID, int>();
        protected Dictionary<NamespaceID, int> cardCounts = new Dictionary<NamespaceID, int>();

        #endregion 属性

        #region 构造器

        public SeedPool()
        {
        }

        #endregion 构造器

        #region 方法
        public bool Contains(NamespaceID id) => cardMaxCounts.ContainsKey(id);

        public void Add(NamespaceID id, int maxCount)
        {
            if (Contains(id))
            {
                throw new ArgumentException($"Attempting to add a card {id} that is already in the Card Pool.");
            }
            cardMaxCounts.Add(id, maxCount);
            cardCounts.Add(id, maxCount);
        }

        public void Remove(NamespaceID id)
        {
            if (!Contains(id))
            {
                throw new ArgumentException($"Attempting to remove a non-exitsing card {id} from the Card Pool.");
            }
            cardMaxCounts.Remove(id);
            cardCounts.Remove(id);
        }
        public void Clear()
        {
            cardMaxCounts.Clear();
            cardCounts.Clear();
        }

        public void Put(NamespaceID id, int count)
        {
            int maxCount = cardMaxCounts[id];
            int added = cardCounts[id] + count;
            if (added > maxCount)
            {
                throw new InvalidOperationException($"Cards of {id} in this Card Pool is full.");
            }
            cardCounts[id] = added;
        }
        public int GetCardPoolSum()
        {
            int sum = 0;
            foreach (var pair in cardCounts)
            {
                int count = Mathf.Max(1, pair.Value);
                sum += count;
            }
            return sum;
        }
        public NamespaceID Draw(int cardIndex)
        {
            int sum = 0;
            foreach (var pair in cardCounts)
            {
                int count = Mathf.Max(1, pair.Value);
                sum += count;

                if (sum > cardIndex)
                {
                    cardCounts[pair.Key]--;
                    return pair.Key;
                }

            }
            throw new NullReferenceException($"Can't draw cards in this Card Pool.");
        }
        public override string ToString()
        {
            string str = "";
            foreach (NamespaceID defRef in cardMaxCounts.Keys)
            {
                str += $"{defRef}: {cardCounts[defRef]}/{cardMaxCounts[defRef]};";
            }
            return str;
        }

        #endregion 方法
    }
}