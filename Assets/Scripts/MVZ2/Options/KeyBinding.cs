using MVZ2.Managers;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Options
{
    public class KeyBindingMeta
    {
        public KeyBindingMeta(NamespaceID id, KeyCode defaultCode, string name)
        {
            ID = id;
            Name = name;
            DefaultCode = defaultCode;
        }
        public NamespaceID ID { get; }
        public string Name { get; }
        public KeyCode DefaultCode { get; }
    }
}
