using System;

namespace PVZEngine.Callbacks
{
    public abstract class CallbackActionBase<T> where T : Delegate
    {
        public T action;
        public int priority;
        public object filter;
        public CallbackActionBase()
        {
        }
        public bool FilterParam(object param)
        {
            return filter == null || filter.Equals(param);
        }
    }
}
