using System.Collections.Generic;
using System;
using MVZ2.Games;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Level;

namespace MVZ2.Modding
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
        public string Namespace { get; }
        private GameDefinitionGroup definitionGroup = new GameDefinitionGroup();
        private CallbackRegistry registry = new CallbackRegistry();
    }
}
