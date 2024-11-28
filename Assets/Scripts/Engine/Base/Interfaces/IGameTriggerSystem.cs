using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(Trigger trigger);
        bool RemoveTrigger(Trigger trigger);
        Trigger[] GetTriggers(NamespaceID callbackID);
        public void RunTriggers(NamespaceID callbackID, params object[] parameters)
        {
            foreach (var trigger in GetTriggers(callbackID))
            {
                trigger.Run(parameters);
            }
        }
    }
}
