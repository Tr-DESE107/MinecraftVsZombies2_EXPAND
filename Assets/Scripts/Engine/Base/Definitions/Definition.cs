using System;
using System.Collections.Generic;
using PVZEngine.Callbacks;

namespace PVZEngine.Base
{
    public abstract class Definition
    {
        public Definition(string nsp, string name)
        {
            id = new NamespaceID(nsp, name);
        }
        public virtual bool TryGetProperty(PropertyKey name, out object value)
        {
            return propertyDict.TryGetProperty(name, out value);
        }
        public virtual object GetProperty(PropertyKey name)
        {
            return propertyDict.GetProperty(name);
        }
        public virtual bool TryGetProperty<T>(PropertyKey name, out T value)
        {
            return propertyDict.TryGetProperty<T>(name, out value);
        }
        public virtual T GetProperty<T>(PropertyKey name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(PropertyKey name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public ITrigger[] GetTriggers()
        {
            return triggers.ToArray();
        }
        public void AddTrigger<TArgs>(CallbackType<TArgs> callbackID, Action<TArgs, CallbackResult> action, int priority = 0, object filter = null)
        {
            triggers.Add(new Trigger<TArgs>(callbackID, action, priority, filter));
        }
        public NamespaceID GetID()
        {
            return id;
        }
        public override string ToString()
        {
            return GetID().ToString();
        }
        public abstract string GetDefinitionType();
        public string Namespace => id.SpaceName;
        public string Name => id.Path;
        private NamespaceID id;
        protected PropertyDictionary propertyDict = new PropertyDictionary();
        protected List<ITrigger> triggers = new List<ITrigger>();
    }
}
