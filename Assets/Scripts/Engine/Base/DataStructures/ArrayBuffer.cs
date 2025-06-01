using System.Collections.Generic;

namespace PVZEngine.Base
{
    public class ArrayBuffer<T>
    {
        public ArrayBuffer(int length)
        {
            array = new T[length];
        }
        public void Clear()
        {
            currentIndex = 0;
        }
        public void CopyFrom(IEnumerable<T> source)
        {
            foreach (var src in source)
            {
                Add(src);
            }
        }
        public void Add(T item)
        {
            if (currentIndex >= array.Length)
            {
                Expand();
            }
            array[currentIndex] = item;
            currentIndex++;
        }
        private void Expand()
        {
            var newArray = new T[array.Length * 2];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            array = newArray;
        }
        public int Count => currentIndex;
        public T this[int index]
        {
            get => array[index];
        }
        private int currentIndex;
        private T[] array;
    }
}
