﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Callbacks;

namespace PVZEngine.Level
{
    public partial class LevelEngine
    {
        #region 公有方法
        public void AddTrigger<TArgs>(Trigger<TArgs> trigger)
        {
            Triggers.AddTrigger(trigger);
            addedTriggers.Add(trigger);
        }
        public void AddTrigger<TArgs>(CallbackType<TArgs> callbackID, Action<TArgs, CallbackResult> action, int priority = 0, object filter = null)
        {
            AddTrigger(new Trigger<TArgs>(callbackID, action, priority, filter));
        }
        public bool RemoveTrigger(ITrigger trigger)
        {
            if (Triggers.RemoveTrigger(trigger))
            {
                addedTriggers.Remove(trigger);
                return true;
            }
            return false;
        }
        public int RemoveTriggers(IEnumerable<ITrigger> triggers)
        {
            int value = 0;
            foreach (var trigger in triggers.ToArray())
            {
                value += RemoveTrigger(trigger) ? 1 : 0;
            }
            return value;
        }
        #endregion

        #region 属性字段
        public IGameTriggerSystem Triggers { get; }
        private List<ITrigger> addedTriggers = new List<ITrigger>();
        #endregion
    }
}
