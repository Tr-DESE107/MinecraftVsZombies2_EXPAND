using System.Collections;
using MVZ2.Managers;
using MVZ2Logic.Games;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using UnityEngine;

namespace MVZ2.GlobalGames
{
    public partial class GlobalGame : IGlobalGame
    {
        public GlobalGame(MainManager main)
        {
            this.main = main;
        }
        public bool IsMobile()
        {
            return main.IsMobile();
        }
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return main.CoroutineManager.StartCoroutine(enumerator);
        }

        #region 属性
        public void SetProperty<T>(PropertyKey<T> name, T value) => propertyDict.SetProperty<T>(name, value);
        public T GetProperty<T>(PropertyKey<T> name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public IPropertyKey[] GetPropertyNames() => propertyDict.GetPropertyNames();
        #endregion

        #region 定义
        public T GetDefinition<T>(string type, NamespaceID id) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(type, id);
        }
        public T[] GetDefinitions<T>(string type) where T : Definition
        {
            return definitionGroup.GetDefinitions<T>(type);
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        public void AddMod(IModLogic mod)
        {
            foreach (var def in mod.GetDefinitions())
            {
                definitionGroup.Add(def);
            }
            foreach (var trigger in mod.GetTriggers())
            {
                callbacks.AddCallback(trigger);
            }
        }
        #endregion

        #region 回调
        public void AddTrigger(ITrigger trigger)
        {
            callbacks.AddCallback(trigger);
        }
        public bool RemoveTrigger(ITrigger trigger)
        {
            return callbacks.RemoveCallback(trigger);
        }

        public void RunCallback<TArgs>(CallbackType<TArgs> callbackType, TArgs args)
        {
            callbacks.RunCallback(callbackType, args);
        }
        public void RunCallbackWithResult<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result)
        {
            callbacks.RunCallbackWithResult(callbackType, args, result);
        }

        public void RunCallbackFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, object filter)
        {
            callbacks.RunCallbackFiltered(callbackType, args, filter);
        }
        public void RunCallbackWithResultFiltered<TArgs>(CallbackType<TArgs> callbackType, TArgs args, CallbackResult result, object filter)
        {
            callbacks.RunCallbackWithResultFiltered(callbackType, args, result, filter);
        }
        #endregion

        public string DefaultNamespace => main.BuiltinNamespace;
        private MainManager main;
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private DefinitionGroup definitionGroup = new DefinitionGroup();
        private CallbackSystem callbacks = new CallbackSystem();
    }
}
