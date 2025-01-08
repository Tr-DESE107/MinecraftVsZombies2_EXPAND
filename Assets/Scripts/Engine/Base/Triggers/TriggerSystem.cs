using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

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
        public Trigger[] GetTriggers(CallbackReference callbackID)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return Array.Empty<Trigger>();
            return triggerList.triggers.ToArray();
        }
        public void RunCallback(CallbackReference callbackID, params object[] args)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return;
            foreach (var trigger in triggerList.triggers)
            {
                trigger.Run(args);
            }
        }
        public void RunCallbackFiltered(CallbackReference callbackID, object filterValue, params object[] args)
        {
            var triggerList = GetTriggerList(callbackID);
            if (triggerList == null)
                return;
            foreach (var trigger in triggerList.triggers)
            {
                if (!trigger.Filter(filterValue))
                    continue;
                trigger.Run(args);
            }
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
            public List<Trigger> triggers = new List<Trigger>();
        }
    }
    public class Trigger
    {
        public Trigger(CallbackReference callbackID, Delegate action, int priorty = 0, object filterValue = null)
        {
            CallbackID = callbackID;
            Action = action;
            cache = WrapDelegate(action);
            Priority = priorty;
            FilterValue = filterValue;
        }
        public void Run(params object[] args)
        {
            Invoke(args);
        }
        public virtual object Invoke(params object[] args)
        {
            return cache.Invoke(Action, args);
        }
        public bool Filter(object value)
        {
            return FilterValue == null || FilterValue.Equals(value);
        }
        private static CachedMethodDelegate WrapDelegate(Delegate del)
        {
            if (del == null)
                throw new ArgumentNullException(nameof(del));

            var type = del.GetType();
            var method = del.GetMethodInfo();

            CreateParamsExpressions(method, out ParameterExpression argsExp, out Expression[] paramsExps);

            var targetExp = Expression.Parameter(typeof(object), "target");
            var castTargetExp = Expression.Convert(targetExp, type);
            var invokeExp = Expression.Invoke(castTargetExp, paramsExps);

            Expression bodyExp;

            if (method.ReturnType != typeof(void))
            {
                bodyExp = Expression.Convert(invokeExp, typeof(object));
            }
            else
            {
                var nullExp = Expression.Constant(null, typeof(object));
                bodyExp = Expression.Block(invokeExp, nullExp);
            }
            var lambdaExp = Expression.Lambda<CachedMethodDelegate>(bodyExp, targetExp, argsExp);
            var lambda = lambdaExp.Compile();
            return lambda;
        }

        private static void CreateParamsExpressions(MethodBase method, out ParameterExpression argsExp, out Expression[] paramsExps)
        {
            argsExp = Expression.Parameter(typeof(object[]), "args");

            var parameters = method.GetParameters();
            paramsExps = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var constExp = Expression.Constant(i, typeof(int));
                var argExp = Expression.ArrayIndex(argsExp, constExp);
                paramsExps[i] = Expression.Convert(argExp, parameters[i].ParameterType);
            }
        }
        private delegate object CachedMethodDelegate(object target, object[] args);
        public CallbackReference CallbackID { get; }
        public Delegate Action { get; }
        public int Priority { get; }
        public object FilterValue { get; }
        private CachedMethodDelegate cache;
    }
}
