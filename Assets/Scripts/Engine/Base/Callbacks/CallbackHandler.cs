using System;
using System.Collections.Generic;

namespace PVZEngine.Callbacks
{
    internal interface ICallbackHandler
    {
        void AddTrigger(ITrigger trigger);
        bool RemoveTrigger(ITrigger trigger);
    }
    internal abstract class CallbackHandlerBase<TTrigger> : ICallbackHandler where TTrigger : ITrigger
    {
        public void AddTrigger(TTrigger trigger)
        {
            lock (syncLock)
            {
                int index = triggers.BinarySearch(trigger, new TriggerPriorityComparer<TTrigger>());
                if (index < 0)
                    index = ~index;
                triggers.Insert(index, trigger);
                triggersSnapshot = null;
            }
        }
        public bool RemoveTrigger(TTrigger trigger)
        {
            lock (syncLock)
            {
                if (triggers.Remove(trigger))
                {
                    triggersSnapshot = null;
                    return true;
                }
                return false;
            }
        }
        void ICallbackHandler.AddTrigger(ITrigger trigger)
        {
            if (trigger is TTrigger t)
            {
                AddTrigger(t);
            }
        }
        bool ICallbackHandler.RemoveTrigger(ITrigger trigger)
        {
            if (trigger is TTrigger t)
            {
                return RemoveTrigger(t);
            }
            return false;
        }
        protected TTrigger[] GetTriggersSnapshot()
        {
            if (triggersSnapshot != null)
                return triggersSnapshot;

            lock (syncLock)
            {
                return triggersSnapshot ??= triggers.ToArray();
            }
        }

        protected readonly object syncLock = new object();
        protected List<TTrigger> triggers = new List<TTrigger>();
        protected volatile TTrigger[] triggersSnapshot;
    }
    internal class CallbackHandler<TArgs> : CallbackHandlerBase<Trigger<TArgs>>
    {
        public void Execute(TArgs args, CallbackResult result)
        {
            var currentTriggers = GetTriggersSnapshot();
            foreach (var trigger in currentTriggers)
            {
                trigger.Action(args, result);
                if (result.IsBreakRequested)
                    break;
            }
        }
        public void ExecuteFiltered(TArgs args, object filter, CallbackResult result)
        {
            var currentTriggers = GetTriggersSnapshot();
            foreach (var trigger in currentTriggers)
            {
                if (!IsFilterMatched(trigger.Filter, filter))
                    continue;

                trigger.Action(args, result);
                if (result.IsBreakRequested)
                    break;
            }
        }
        private bool IsFilterMatched(object triggerFilter, object callbackFilter)
        {
            if (triggerFilter == null || callbackFilter == null)
                return true;
            return triggerFilter.Equals(callbackFilter);
        }
    }
    internal class TriggerPriorityComparer<T> : IComparer<T> where T : ITrigger
    {
        public int Compare(T x, T y)
            => x.Priority.CompareTo(y.Priority);
    }
}
