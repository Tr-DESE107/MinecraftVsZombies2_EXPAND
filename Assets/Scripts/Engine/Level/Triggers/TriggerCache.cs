using System;

namespace PVZEngine.Level.Triggers
{
    public class TriggerCache
    {
        public TriggerCache(NamespaceID callbackId, Delegate action, int priority = 0, object filterValue = null)
        {
            CallbackID = callbackId;
            Action = action;
            Priority = priority;
            FilterValue = filterValue;
        }
        public NamespaceID CallbackID { get; }
        public Delegate Action { get; }
        public int Priority { get; }
        public object FilterValue { get; }
    }
}
