using System;
using PVZEngine;
using PVZEngine.Triggers;

namespace MVZ2.Games
{
    public partial class Game : IGameTriggerSystem
    {
        #region 公有方法
        public void AddTrigger(ITrigger trigger)
        {
            triggerSystem.AddTrigger(trigger);
        }
        public bool RemoveTrigger(ITrigger trigger)
        {
            return triggerSystem.RemoveTrigger(trigger);
        }
        public Trigger<T>[] GetTriggers<T>(CallbackReference<T> callbackID) where T: Delegate
        {
            return triggerSystem.GetTriggers<T>(callbackID);
        }
        #endregion

        #region 属性字段
        private TriggerSystem triggerSystem = new TriggerSystem();
        #endregion
    }
}