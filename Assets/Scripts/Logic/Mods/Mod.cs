using System;
using MVZ2Logic.Saves;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Triggers;

namespace MVZ2Logic.Modding
{
    public abstract class Mod : IContentProvider, IModLogic
    {
        public Mod(string nsp)
        {
            Namespace = nsp;
        }
        public void Load()
        {
            AddCallbacks();
        }
        public void Unload()
        {
            RemoveCallbacks();
        }
        public virtual void PostGameInit() { }
        public abstract ModSaveData CreateSaveData();
        public abstract ModSaveData LoadSaveData(string json);
        protected void AddDefinition(Definition def)
        {
            definitionGroup.Add(def);
        }
        public T GetDefinition<T>(NamespaceID defRef) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(defRef);
        }
        public T[] GetDefinitions<T>() where T : Definition
        {
            return definitionGroup.GetDefinitions<T>();
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        public void RegisterCallback<TEntry, TDelegate>(CallbackListBase<TEntry, TDelegate> callbackList, TDelegate action, int priority = 0, object filter = null)
            where TDelegate : Delegate
            where TEntry : CallbackActionBase<TDelegate>, new()
        {
            registry.RegisterCallback(callbackList, action, priority, filter);
        }
        private void AddCallbacks()
        {
            registry.AddCallbacks();
        }
        private void RemoveCallbacks()
        {
            registry.RemoveCallbacks();
        }
        public void AddTrigger<T>(NamespaceID callbackID, T action, int priority = 0, object filterValue = null) where T : Delegate
        {
            Global.Game.AddTrigger(new ModTrigger(callbackID, action, priority, filterValue));
        }
        public string Namespace { get; }
        private DefinitionGroup definitionGroup = new DefinitionGroup();
        private CallbackRegistry registry = new CallbackRegistry();
    }
    public class ModTrigger : Trigger
    {
        public ModTrigger(NamespaceID callbackID, Delegate action, int priority = 0, object filterValue = null) : base(callbackID, action, priority, filterValue)
        {
        }

        public override object Invoke(params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
