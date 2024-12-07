using System;
using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(Trigger trigger);
        bool RemoveTrigger(Trigger trigger);
        Trigger[] GetTriggers(CallbackReference callbackID);
        public void RunCallback(CallbackReference callbackID, params object[] parameters);
        public void RunCallbackFiltered(CallbackReference callbackID, object filterValue, params object[] parameters);
    }
}
