using System.Collections;
using System.Collections.Generic;

namespace PVZEngine.Base
{
    public class UpdateList<TKey> : IEnumerable<TKey>
    {
        public UpdateList()
        {

        }
        public UpdateList(IEnumerable<TKey> elements)
        {
            this.elements.AddRange(elements);
        }
        public virtual void Update(IEnumerable<TKey> validSources)
        {
            UpdateElements(validSources);
        }
        protected void UpdateElements(IEnumerable<TKey> validSources)
        {
            toAddBuffer.Clear();
            foreach (var source in validSources)
            {
                toAddBuffer.Add(source);
            }
            // 检查目前的元素列表，将不符合条件的元素加到移除列表中，将符合条件的元素保留，剩下的就是需要新增的元素。
            toRemoveBuffer.Clear();
            foreach (var key in elements)
            {
                if (toAddBuffer.Contains(key))
                {
                    // 符合条件并且目前在表中。
                    // 保留而非新添加到表中。
                    toAddBuffer.Remove(key);
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
            foreach (var id in toAddBuffer)
            {
                elements.Add(id);
            }
        }

        public IEnumerator<TKey> GetEnumerator()
        {
            return ((IEnumerable<TKey>)elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)elements).GetEnumerator();
        }
        private List<TKey> toAddBuffer = new List<TKey>();
        private List<TKey> toRemoveBuffer = new List<TKey>();
        private List<TKey> elements = new List<TKey>();
    }
}
