using System.Collections.Generic;

namespace PVZEngine
{
    public class PropertyDictionary
    {
        public void SetProperty(string name, object value)
        {
            propertyDict[name] = value;
        }
        public object GetProperty(string name)
        {
            if (propertyDict.TryGetValue(name, out var prop))
                return prop;
            return null;
        }
        public T GetProperty<T>(string name)
        {
            var prop = GetProperty(name);
            if (prop is T tProp)
            {
                return tProp;
            }
            return default;
        }
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
    }
}
