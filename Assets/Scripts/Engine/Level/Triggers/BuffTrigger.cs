using PVZEngine.Buffs;

namespace PVZEngine.Triggers
{
    public class BuffTrigger : Trigger
    {
        public BuffTrigger(Buff buff, TriggerCache cache) : base(cache.CallbackID, cache.Action, cache.Priority, cache.FilterValue)
        {
            this.buff = buff;
        }
        public override object Invoke(params object[] args)
        {
            var arguments = new object[args.Length + 1];
            arguments[0] = buff;
            args.CopyTo(arguments, 1);
            return Action.DynamicInvoke(arguments);
        }
        public Buff buff;
    }
}
