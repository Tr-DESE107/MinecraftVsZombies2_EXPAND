using System;
using System.Collections.Generic;

namespace PVZEngine
{
    [Serializable]
    public class SerializablePropertyDictionary : Dictionary<string, object>
    {
        public SerializablePropertyDictionary() { }
        public SerializablePropertyDictionary(IDictionary<string, object> properties)
        {
            foreach (var pair in properties)
            {
                Add(pair.Key, pair.Value);
            }
        }
    }
}
