using System;
using System.Collections.Generic;

namespace PVZEngine.Triggers
{
    public class TriggerSystem
    {
        public void AddTrigger(Trigger trigger)
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
        public bool RemoveTrigger(Trigger trigger)
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
        public Trigger[] GetTriggers(NamespaceID callbackID)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return Array.Empty<Trigger>();
            return triggerList.triggers.ToArray();
        }
        public void RunCallback(NamespaceID callbackID, params object[] args)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return;
            foreach (var trigger in triggerList.triggers)
            {
                trigger.Run(args);
            }
        }
        public void RunCallbackFiltered(NamespaceID callbackID, object filterValue, params object[] args)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return;
            foreach (var trigger in triggerList.triggers)
            {
                if (trigger.FilterValue != null && !trigger.FilterValue.Equals(filterValue))
                    continue;
                trigger.Run(args);
            }
        }
        private EventTriggerList GetTriggerList(NamespaceID callbackID)
        {
            return triggerLists.Find(l => l.callbackID == callbackID);
        }
        private List<EventTriggerList> triggerLists = new List<EventTriggerList>();
        private class EventTriggerList
        {
            public EventTriggerList(NamespaceID callbackID)
            {
                this.callbackID = callbackID;
            }
            public NamespaceID callbackID;
            public List<Trigger> triggers = new List<Trigger>();
        }
    }
    public abstract class Trigger
    {
        public Trigger(NamespaceID callbackID, Delegate action, int priorty = 0, object filterValue = null)
        {
            CallbackID = callbackID;
            Action = action;
            Priority = priorty;
            FilterValue = filterValue;
        }
        public void Run(params object[] args)
        {
            Invoke(args);
        }
        public virtual object Invoke(params object[] args)
        {
            return Action?.DynamicInvoke(args);
        }
        public NamespaceID CallbackID { get; }
        public Delegate Action { get; }
        public int Priority { get; }
        public object FilterValue { get; }
    }
}
