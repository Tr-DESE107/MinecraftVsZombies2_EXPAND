﻿using System;
using System.Collections.Generic;
using System.Linq;
using PVZEngine.Base;

namespace PVZEngine
{
    public class DefinitionGroup
    {
        public DefinitionGroup()
        {
        }
        public void Add(Definition definition)
        {
            if (definition == null)
                return;
            var type = definition.GetDefinitionType();
            if (!lists.TryGetValue(type, out var list))
            {
                list = new DefinitionList();
                lists.Add(type, list);
            }
            list.Add(definition);
        }
        public T GetDefinition<T>(string type, NamespaceID id) where T : Definition
        {
            if (!lists.TryGetValue(type, out var list))
            {
                return default;
            }
            return list.GetDefinition<T>(id);
        }
        public T[] GetDefinitions<T>(string type) where T : Definition
        {
            if (!lists.TryGetValue(type, out var list))
            {
                return default;
            }
            return list.GetAllDefinitions<T>();
        }
        public Definition[] GetDefinitions()
        {
            return lists.SelectMany(l => l.Value.GetAllDefinitions()).ToArray();
        }
        protected Dictionary<string, DefinitionList> lists = new Dictionary<string, DefinitionList>();
    }
}
