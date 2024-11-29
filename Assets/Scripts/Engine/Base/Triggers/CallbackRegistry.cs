using System.Collections.Generic;
using PVZEngine.Triggers;

namespace PVZEngine.Base
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
        public void AddTrigger(Trigger trigger)
        {
            triggers.Add(trigger);
        }
        #endregion

        #region 属性字段
        public IGameTriggerSystem Triggers { get; }
        protected List<Trigger> triggers = new List<Trigger>();
        #endregion
    }
}
