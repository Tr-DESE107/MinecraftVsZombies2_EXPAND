using System;
using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(ITrigger trigger);
        bool RemoveTrigger(ITrigger trigger);
        Trigger<T>[] GetTriggers<T>(CallbackReference<T> callbackID) where T : Delegate;
    }
}
