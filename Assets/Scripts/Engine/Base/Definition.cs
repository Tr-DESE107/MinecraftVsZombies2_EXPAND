using System;
using System.Collections.Generic;
using PVZEngine.Triggers;

namespace PVZEngine.Base
{
    public abstract class Definition
    {
        public Definition(string nsp, string name)
        {
            Namespace = nsp;
            Name = name;
        }
        public virtual bool TryGetProperty<T>(string name, out T value)
        {
            return propertyDict.TryGetProperty<T>(name, out value);
        }
        public virtual T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
        }
        public string[] GetPropertyNames()
        {
            return propertyDict.GetPropertyNames();
        }
        public Trigger[] GetTriggers()
        {
            return triggers.ToArray();
        }
        public void AddTrigger<T>(CallbackReference<T> callbackID, T action, int priority = 0, object filter = null) where T : Delegate
        {
            triggers.Add(new Trigger(callbackID, action, priority, filter));
        }
        public NamespaceID GetID()
        {
            return new NamespaceID(Namespace, Name);
        }
        public override string ToString()
        {
            return GetID().ToString();
        }
        public string Namespace { get; set; }
        public string Name { get; set; }
        protected PropertyDictionary propertyDict = new PropertyDictionary();
        protected List<Trigger> triggers = new List<Trigger>();
    }
}
