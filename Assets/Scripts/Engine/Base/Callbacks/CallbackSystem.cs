using System.Collections.Concurrent;

namespace PVZEngine.Callbacks
{
    public class CallbackSystem : ICallbackRunner
    {
        public void RunCallback<TArgs>(CallbackType<TArgs> callbackType, TArgs args)
        {
            RunCallbackWithResult(callbackType, args, new CallbackResult());
        }
        public void RunCallbackWithResult<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result)
        {
            if (handlers.TryGetValue(callbackType, out var handler))
            {
                var callbackHandler = (CallbackHandler<TArgs>)handler;
                callbackHandler.Execute(args, result);
            }
        }
        public void RunCallbackFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, object filter)
        {
            RunCallbackWithResultFiltered(callbackType, args, new CallbackResult(), filter);
        }
        public void RunCallbackWithResultFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result, object filter)
        {
            if (handlers.TryGetValue(callbackType, out var handler))
            {
                var callbackHandler = (CallbackHandler<TArgs>)handler;
                callbackHandler.ExecuteFiltered(args, filter, result);
            }
        }

        public void AddCallback(ITrigger trigger)
        {
            var handler = handlers.GetOrAdd(trigger.Type, type => type.CreateHandler());
            handler.AddTrigger(trigger);
        }
        public bool RemoveCallback(ITrigger trigger)
        {
            if (handlers.TryGetValue(trigger.Type, out var handler))
            {
                var callbackHandler = handler;
                return callbackHandler.RemoveTrigger(trigger);
            }
            return false;
        }

        private readonly ConcurrentDictionary<ICallbackType, ICallbackHandler> handlers = new ConcurrentDictionary<ICallbackType, ICallbackHandler>();
    }
}
