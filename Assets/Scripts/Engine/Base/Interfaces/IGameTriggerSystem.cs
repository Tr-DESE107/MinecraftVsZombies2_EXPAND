using PVZEngine.Callbacks;

namespace PVZEngine
{
    public interface IGameTriggerSystem : ICallbackRunner
    {
        void AddTrigger(ITrigger trigger);
        bool RemoveTrigger(ITrigger trigger);
    }
}
