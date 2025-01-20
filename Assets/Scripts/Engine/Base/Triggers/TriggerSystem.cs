using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tools;

namespace PVZEngine.Triggers
{
    public class TriggerSystem
    {
        public void AddTrigger(ITrigger trigger)
        {
            if (trigger == null)
                return;
            var callbackID = trigger.CallbackID;
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
            {
                triggerList = new EventTriggerList(callbackID);
                triggerLists.Add(triggerList);
            }
            triggerList.triggers.Add(trigger);
            triggerList.triggers.Sort((t1, t2) => t1.Priority.CompareTo(t2.Priority));
        }
        public bool RemoveTrigger(ITrigger trigger)
        {
            if (trigger == null)
                return false;
            var callbackID = trigger.CallbackID;
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return false;
            bool removed = triggerList.triggers.Remove(trigger);
            if (triggerList.triggers.Count <= 0)
            {
                triggerLists.Remove(triggerList);
            }
            return removed;
        }
        public void GetTriggers(CallbackReference callbackID, List<ITrigger> triggers)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return;
            triggers.AddRange(triggerList.triggers);
        }
        private EventTriggerList GetTriggerList(CallbackReference callbackID)
        {
            foreach (var triggerList in triggerLists)
            {
                if (triggerList.callbackID == callbackID)
                    return triggerList;
            }
            return null;
        }
        private List<EventTriggerList> triggerLists = new List<EventTriggerList>();
        private class EventTriggerList
        {
            public EventTriggerList(CallbackReference callbackID)
            {
                this.callbackID = callbackID;
            }
            public CallbackReference callbackID;
            public List<ITrigger> triggers = new List<ITrigger>();
        }
    }
    public interface ITrigger
    {
        CallbackReference CallbackID { get; }
        int Priority { get; }
        bool Filter(object value);
    }
    public class Trigger<T> : ITrigger where T: Delegate
    {
        public Trigger(CallbackReference<T> callbackID, T action, int priorty = 0, object filterValue = null)
        {
            CallbackID = callbackID;
            Action = action;
            Priority = priorty;
            FilterValue = filterValue;
        }
        public void Run(Action<T> runner)
        {
            runner?.Invoke(Action);
        }
        public virtual TResult Invoke<TResult>(Func<T, TResult> runner)
        {
            return runner.Invoke(Action);
        }
        public bool Filter(object value)
        {
            if (value == null)
                return true;
            return FilterValue == null || FilterValue.Equals(value);
        }
        CallbackReference ITrigger.CallbackID => CallbackID;
        public CallbackReference<T> CallbackID { get; }
        public T Action { get; }
        public int Priority { get; }
        public object FilterValue { get; }
    }
}
