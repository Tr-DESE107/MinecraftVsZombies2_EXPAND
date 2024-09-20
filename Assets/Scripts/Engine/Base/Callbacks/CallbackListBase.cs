using System;
using System.Collections.Generic;
using System.Linq;

namespace PVZEngine.Callbacks
{
    public abstract class CallbackListBase<TEntry, TDelegate>
        where TDelegate : Delegate
        where TEntry : CallbackActionBase<TDelegate>, new()
    {
        public void Add(TDelegate action, int priority = 0, object filter = null)
        {
            var callback = new TEntry();
            callback.action = action;
            callback.priority = priority;
            callback.filter = filter;
            Add(callback);
        }
        public bool Remove(TDelegate action)
        {
            return Remove(callbacks.FirstOrDefault(f => f.action == action));
        }
        public TEntry[] GetFilteredCallbacks(object filter)
        {
            return callbacks.Where(c => c.FilterParam(filter)).ToArray();
        }
        protected void Add(TEntry function)
        {
            callbacks.Add(function);
            callbacks.Sort((x, y) => x.priority.CompareTo(y.priority));
        }
        protected bool Remove(TEntry function)
        {
            return callbacks.Remove(function);
        }
        protected List<TEntry> callbacks = new List<TEntry>();
    }
}
