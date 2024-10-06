using System;
using System.Collections.Generic;
using PVZEngine.Triggers;

namespace PVZEngine.Level.Triggers
{
    public class TriggerCacheList
    {
        public TriggerCache[] GetTriggerCaches()
        {
            return triggerCaches.ToArray();
        }
        public void Add(TriggerCache cache)
        {
            triggerCaches.Add(cache);
        }
        private List<TriggerCache> triggerCaches = new List<TriggerCache>();
    }
}
