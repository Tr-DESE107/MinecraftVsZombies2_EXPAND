﻿using System.Collections.Generic;

namespace PVZEngine.Callbacks
{
    public class CallbackRegistry
    {
        public CallbackRegistry(IGameTriggerSystem triggers)
        {
            Triggers = triggers;
        }
        #region 公有方法
        public void ApplyCallbacks()
        {
            foreach (var trigger in triggers)
            {
                Triggers.AddTrigger(trigger);
            }
        }
        public void RevertCallbacks()
        {
            foreach (var trigger in triggers)
            {
                Triggers.RemoveTrigger(trigger);
            }
        }
        public void AddTrigger(ITrigger trigger)
        {
            triggers.Add(trigger);
        }
        #endregion

        #region 属性字段
        public IGameTriggerSystem Triggers { get; }
        protected List<ITrigger> triggers = new List<ITrigger>();
        #endregion
    }
}
