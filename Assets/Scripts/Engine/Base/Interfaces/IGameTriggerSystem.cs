using PVZEngine.Triggers;

namespace PVZEngine
{
    public interface IGameTriggerSystem
    {
        void AddTrigger(Trigger trigger);
        bool RemoveTrigger(Trigger trigger);
        Trigger[] GetTriggers(NamespaceID callbackID);
        public void RunCallback(NamespaceID callbackID, params object[] parameters);
        public void RunCallbackFiltered(NamespaceID callbackID, object filterValue, params object[] parameters);
        public void RunCallback(CallbackReference callbackID, params object[] parameters) => RunCallback(callbackID.ID, parameters);
        public void RunCallbackFiltered(CallbackReference callbackID, object filterValue, params object[] parameters) => RunCallbackFiltered(callbackID.ID, filterValue, parameters);
    }
}
