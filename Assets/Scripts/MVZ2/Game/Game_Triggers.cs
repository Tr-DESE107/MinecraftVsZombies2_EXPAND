using System.Collections.Generic;
using PVZEngine;
using PVZEngine.Triggers;

namespace MVZ2.Games
{
    public partial class Game : IGameTriggerSystem
    {
        #region 公有方法
        public void AddTrigger(Trigger trigger)
        {
            triggerSystem.AddTrigger(trigger);
        }
        public bool RemoveTrigger(Trigger trigger)
        {
            return triggerSystem.RemoveTrigger(trigger);
        }
        public void AddTriggers(IEnumerable<Trigger> triggers)
        {
            foreach (var trigger in triggers)
            {
                AddTrigger(trigger);
            }
        }
        public int RemoveTriggers(IEnumerable<Trigger> triggers)
        {
            int value = 0;
            foreach (var trigger in triggers)
            {
                value += RemoveTrigger(trigger) ? 1 : 0;
            }
            return value;
        }
        public Trigger[] GetTriggers(NamespaceID callbackID)
        {
            return triggerSystem.GetTriggers(callbackID);
        }
        public void RunCallback(NamespaceID callbackID, params object[] parameters)
        {
            triggerSystem.RunCallback(callbackID, parameters);
        }
        public void RunCallbackFiltered(NamespaceID callbackID, object filterValue, params object[] parameters)
        {
            triggerSystem.RunCallbackFiltered(callbackID, filterValue, parameters);
        }
        #endregion

        #region 属性字段
        private TriggerSystem triggerSystem = new TriggerSystem();
        #endregion
    }
}