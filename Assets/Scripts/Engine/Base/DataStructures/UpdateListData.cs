using System.Collections;
using System.Collections.Generic;

namespace PVZEngine.Base
{
    public abstract class UpdateListData<TKey, TSource, TData> : IEnumerable<KeyValuePair<TKey, TData>>
    {
        public virtual void Update(IEnumerable<TSource> validSources)
        {
            UpdateElements(validSources);
        }
        public bool TryGetValue(TKey key, out TData data)
        {
            return elements.TryGetValue(key, out data);
        }
        public IEnumerable<TKey> GetKeys()
        {
            return elements.Keys;
        }
        public IEnumerable<TData> GetValues()
        {
            return elements.Values;
        }
        protected void UpdateElements(IEnumerable<TSource> validSources)
        {
            // 将元素列表转换为Dictionary，以便快速查找。
            toAddBuffer.Clear();
            foreach (var source in validSources)
            {
                toAddBuffer.Add(GetKey(source), source);
            }
            // 检查目前的元素列表，将不符合条件的元素加到移除列表中，将符合条件的元素保留，剩下的就是需要新增的元素。
            toRemoveBuffer.Clear();
            foreach (var pair in elements)
            {
                var key = pair.Key;
                if (toAddBuffer.TryGetValue(key, out var source))
                {
                    // 符合条件并且目前在表中。
                    // 保留而非新添加到表中。
                    toAddBuffer.Remove(key);
                    // 更新元素。
                    UpdateElement(source, pair.Value);
                }
                else
                {
                    // 不符合条件。
                    toRemoveBuffer.Add(key);
                }
            }
            // 移除不符合条件的元素。
            foreach (var id in toRemoveBuffer)
            {
                elements.Remove(id);
            }
            // 添加新的符合条件的元素。
            foreach (var pair in toAddBuffer)
            {
                var source = pair.Value;
                var data = CreateData(source);
                elements.Add(pair.Key, data);
                UpdateElement(source, data);
            }
        }
        public void Add(TKey key, TData data)
        {
            elements.Add(key, data);
        }
        public IEnumerator<KeyValuePair<TKey, TData>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TData>>)elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)elements).GetEnumerator();
        }
        protected abstract TKey GetKey(TSource source);
        protected abstract TData CreateData(TSource source);
        protected virtual void UpdateElement(TSource source, TData data) { }
        private Dictionary<TKey, TSource> toAddBuffer = new Dictionary<TKey, TSource>();
        private List<TKey> toRemoveBuffer = new List<TKey>();
        private Dictionary<TKey, TData> elements = new Dictionary<TKey, TData>();

    }
}
