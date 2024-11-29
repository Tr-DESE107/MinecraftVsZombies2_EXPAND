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
        public Trigger[] GetTriggers(CallbackReference callbackID)
        {
            return triggerSystem.GetTriggers(callbackID);
        }
        public void RunCallback(CallbackReference callbackID, params object[] parameters)
        {
            triggerSystem.RunCallback(callbackID, parameters);
        }
        public void RunCallbackFiltered(CallbackReference callbackID, object filterValue, params object[] parameters)
        {
            triggerSystem.RunCallbackFiltered(callbackID, filterValue, parameters);
        }
        #endregion

        #region 属性字段
        private TriggerSystem triggerSystem = new TriggerSystem();
        #endregion
    }
}