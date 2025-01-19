using System.Collections.Generic;

namespace Tools
{
    public abstract class ObjectPool<T> where T : IPoolable
    {
        public T Get()
        {
            if (pool.Count <= 0)
            {
                return Create();
            }
            return pool.Pop();
        }
        public void Release(T obj)
        {
            obj.Reset();
            pool.Push(obj);
        }
        protected abstract T Create();
        public Stack<T> pool = new Stack<T>();
    }
}
