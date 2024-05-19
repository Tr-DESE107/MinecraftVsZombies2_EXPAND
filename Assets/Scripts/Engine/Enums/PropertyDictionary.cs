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
            return ToGeneric<T>(GetProperty(name));
        }
        public static T ToGeneric<T>(object value)
        {
            if (value is int intValue && typeof(T) == typeof(float))
            {
                var floatValue = (float)intValue;
                if (floatValue is T floatResult)
                    return floatResult;
            }
            if (value is T tProp)
            {
                return tProp;
            }
            return default;
        }
        private Dictionary<string, object> propertyDict = new Dictionary<string, object>();
    }
}
