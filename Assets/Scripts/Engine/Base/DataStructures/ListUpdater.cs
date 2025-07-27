using System;
using System.Collections.Generic;

namespace PVZEngine.Base
{
    public class ListUpdater<T>
    {
        public void Update(IEnumerable<T> targets, IEnumerable<T> current, Action<T> addAction, Action<T> removeAction, Action<T> updateAction = null)
        {
            // 将所有合法的元素添加到待添加列表中。
            addBuffer.Clear();
            addBuffer.AddRange(targets);

            // 检查目前的列表，将不符合条件的元素加到移除列表中，将符合条件的元素保留，剩下的就是需要新增的元素。
            removeBuffer.Clear();
            foreach (var element in current)
            {
                if (addBuffer.Contains(element))
                {
                    // 符合条件并且目前在表中。
                    // 保留而非新添加到表中。
                    addBuffer.Remove(element);
                    updateAction?.Invoke(element);
                }
                else
                {
                    // 不符合条件。
                    removeBuffer.Add(element);
                }
            }
            // 移除不符合条件的元素。
            foreach (var element in removeBuffer)
            {
                removeAction?.Invoke(element);
            }
            // 添加新的符合条件的元素。
            foreach (var element in addBuffer)
            {
                addAction?.Invoke(element);
                updateAction?.Invoke(element);
            }
        }
        private List<T> addBuffer = new List<T>();
        private List<T> removeBuffer = new List<T>();
    }
}
