using System;
using System.Reflection;
using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;

namespace MVZ2Logic.Modding
{
    public abstract class Mod : IGameContent, IModLogic
    {
        public Mod(string nsp)
        {
            Namespace = nsp;
            triggers = new CallbackRegistry(Global.Game);
        }
        public virtual void Init(IGame game, Assembly[] assemblies)
        {

        }
        public virtual void LateInit(IGame game)
        {

        }
        public virtual void PostGameInit() { }
        public virtual void PostReloadMods(IGame game)
        {
            foreach (var definition in definitionGroup.GetDefinitions())
            {
                if (definition is ICachedDefinition cached)
                {
                    cached.ClearCaches();
                    cached.CacheContents(game);
                }
            }
        }
        public void Load()
        {
            ApplyCallbacks();
        }
        public void Unload()
        {
            RevertCallbacks();
        }
        public abstract ModSaveData CreateSaveData();
        public abstract ModSaveData LoadSaveData(string json);
        protected void AddDefinition(Definition def)
        {
            definitionGroup.Add(def);
            foreach (var trigger in def.GetTriggers())
            {
                triggers.AddTrigger(trigger);
            }
        }
        public T GetDefinition<T>(string type, NamespaceID defRef) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(type, defRef);
        }
        public T[] GetDefinitions<T>(string type) where T : Definition
        {
            return definitionGroup.GetDefinitions<T>(type);
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        private void ApplyCallbacks()
        {
            triggers.ApplyCallbacks();
        }
        private void RevertCallbacks()
        {
            triggers.RevertCallbacks();
        }
        public void AddTrigger<TArgs>(CallbackType<TArgs> callbackID, Action<TArgs, CallbackResult> action, int priority = 0, object filter = null)
        {
            triggers.AddTrigger(new Trigger<TArgs>(callbackID, action, priority, filter));
        }
        public string Namespace { get; }
        private DefinitionGroup definitionGroup = new DefinitionGroup();
        protected CallbackRegistry triggers;
    }
}
