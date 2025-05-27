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
        public virtual bool TryGetProperty<T>(PropertyKey<T> name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public virtual T GetProperty<T>(PropertyKey<T> name) => propertyDict.GetProperty<T>(name);
        public void SetProperty<T>(PropertyKey<T> name, T value) => propertyDict.SetProperty(name, value);
        public void SetPropertyObject(IPropertyKey name, object value) => propertyDict.SetPropertyObject(name, value);
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
