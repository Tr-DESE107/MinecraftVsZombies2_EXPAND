using System;

namespace PVZEngine.Callbacks
{
    public class Trigger<TArgs> : ITrigger
    {
        public CallbackType<TArgs> Type { get; }
        ICallbackType ITrigger.Type => Type;
        public Action<TArgs, CallbackResult> Action { get; }
        public int Priority { get; }
        public object Filter { get; }

        public Trigger(CallbackType<TArgs> type, Action<TArgs, CallbackResult> action, int priority, object filter)
        {
            Type = type;
            Action = action;
            Priority = priority;
            Filter = filter;
        }
    }
}
