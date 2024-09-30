using System;
using System.Collections.Generic;
using PVZEngine.Callbacks;

namespace PVZEngine.Base
{
    public class CallbackRegistry
    {
        public void AddCallbacks()
        {
            foreach (var action in addCallbackActions)
            {
                action?.Invoke();
            }
        }
        public void RemoveCallbacks()
        {
            foreach (var action in removeCallbackActions)
            {
                action?.Invoke();
            }
        }
        public void RegisterCallback<TEntry, TDelegate>(CallbackListBase<TEntry, TDelegate> callbackList, TDelegate action, int priority = 0, object filter = null)
            where TDelegate : Delegate
            where TEntry : CallbackActionBase<TDelegate>, new()
        {
            addCallbackActions.Add(() => callbackList.Add(action, priority, filter));
            removeCallbackActions.Add(() => callbackList.Remove(action));
        }
        private List<Action> addCallbackActions = new List<Action>();
        private List<Action> removeCallbackActions = new List<Action>();
    }
}
