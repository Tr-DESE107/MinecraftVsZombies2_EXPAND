using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface ITriggerProvider
    {
        void AddTrigger(Trigger trigger);
        bool RemoveTrigger(Trigger trigger);
    }
}
