using System;

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
            var triggerList = triggers.GetTriggers<T>(callbackID);
            if (triggerList == null)
                return;
            foreach (var trigger in triggerList)
            {
                if (source != null && source.IsInterrupted)
                    return;
                if (!trigger.Filter(filterValue))
                    continue;
                trigger.Run(runner);
            }
        }
    }
}
