using System;
using PVZEngine.Callbacks;
using PVZEngine.Triggers;

namespace PVZEngine.Level.Triggers
{
    public class BuffTrigger : Trigger
    {
        public BuffTrigger(Buff buff, TriggerCache cache) : base(cache.CallbackID, cache.Action, cache.Priority, cache.FilterValue)
        {
            this.buff = buff;
        }
        public override object Invoke(params object[] args)
        {
            return Action.DynamicInvoke(buff, args);
        }
        public Buff buff;
    }
}
