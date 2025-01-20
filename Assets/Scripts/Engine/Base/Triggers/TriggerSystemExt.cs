using System;
using System.Collections.Generic;
using Tools;

namespace PVZEngine.Triggers
{
    public static class TriggerSystemExt
    {
        public static void RunCallback<T>(this IGameTriggerSystem triggers, CallbackReference<T> callbackID, Action<T> runner) where T : Delegate
        {
            triggers.RunCallback(callbackID, null, runner);
        }
        public static void RunCallback<T>(this IGameTriggerSystem triggers, CallbackReference<T> callbackID, IInterruptSource source, Action<T> runner) where T : Delegate
        {
            triggers.RunCallbackFiltered(callbackID, null, null, runner);
        }
        public static void RunCallbackFiltered<T>(this IGameTriggerSystem triggers, CallbackReference<T> callbackID, object filterValue, Action<T> runner) where T : Delegate
        {
            triggers.RunCallbackFiltered(callbackID, filterValue, null, runner);
        }
        public static void RunCallbackFiltered<T>(this IGameTriggerSystem triggers, CallbackReference<T> callbackID, object filterValue, IInterruptSource source, Action<T> runner) where T : Delegate
        {
            var buffer = pool.Get();
            triggers.GetTriggers(callbackID, buffer.list);
            foreach (var trigger in buffer.list)
            {
                if (source != null && source.IsInterrupted)
                    return;
                if (!trigger.Filter(filterValue))
                    continue;
                if (trigger is not Trigger<T> tTrigger)
                    continue;
                tTrigger.Run(runner);
            }
            pool.Release(buffer);
        }
        private static TriggerBufferPool pool = new TriggerBufferPool();
    }
    internal class TriggerBuffer : IPoolable
    {
        public void Reset()
        {
            list.Clear();
        }
        public List<ITrigger> list = new List<ITrigger>();
    }
    internal class TriggerBufferPool : ObjectPool<TriggerBuffer>
    {
        protected override TriggerBuffer Create()
        {
            return new TriggerBuffer();
        }
    }
}
