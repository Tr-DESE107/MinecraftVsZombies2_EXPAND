using System;
using System.Collections.Generic;
using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(ITrigger trigger);
        bool RemoveTrigger(ITrigger trigger);
        void GetTriggers(CallbackReference callbackID, List<ITrigger> results);
    }
}
