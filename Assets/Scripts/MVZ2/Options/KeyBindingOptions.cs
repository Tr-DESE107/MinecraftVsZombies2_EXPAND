﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Options
{
    public class KeyBindingOptions
    {
        public bool TryGetKeyBinding(NamespaceID hotkey, out KeyCode code)
        {
            return bindings.TryGetValue(hotkey, out code);
        }
        public void SetKeyBinding(NamespaceID hotkey, KeyCode code)
        {
            bindings[hotkey] = code;
        }
        public void Reset()
        {
            bindings.Clear();
        }
        public SerializableKeyBindingOptions ToSerializable()
        {
            return new SerializableKeyBindingOptions()
            {
                bindings = bindings.ToDictionary(p => p.Key, p => (int)p.Value)
            };
        }
        public void LoadFromSerializable(SerializableKeyBindingOptions seri)
        {
            if (seri == null)
                return;
            bindings = seri.bindings.ToDictionary(p => p.Key, p => (KeyCode)p.Value);
        }

        private Dictionary<NamespaceID, KeyCode> bindings = new Dictionary<NamespaceID, KeyCode>();
    }
    [Serializable]
    public class SerializableKeyBindingOptions
    {
        public Dictionary<NamespaceID, int> bindings;
    }
}
