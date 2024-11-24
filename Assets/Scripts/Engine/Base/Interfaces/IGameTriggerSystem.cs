using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(Trigger trigger);
        bool RemoveTrigger(Trigger trigger);
        Trigger[] GetTriggers(NamespaceID callbackID);
    }
}
