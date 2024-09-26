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
        protected void AddCallback<TEntry, TDelegate>(CallbackListBase<TEntry, TDelegate> callbackList, TDelegate action, int priority = 0, object filter = null)
            where TDelegate : Delegate
            where TEntry : CallbackActionBase<TDelegate>, new()
        {
            addCallbackActions.Add(() => callbackList.Add(action, priority, filter));
            removeCallbackActions.Add(() => callbackList.Remove(action));
        }
        private void AddCallbacks()
        {
            foreach (var action in addCallbackActions)
            {
                action?.Invoke();
            }
        }
        private void RemoveCallbacks()
        {
            foreach (var action in removeCallbackActions)
            {
                action?.Invoke();
            }
        }
        public string Namespace { get; }
        private GameDefinitionGroup definitionGroup = new GameDefinitionGroup();
        private List<Action> addCallbackActions = new List<Action>();
        private List<Action> removeCallbackActions = new List<Action>();

    }
}
