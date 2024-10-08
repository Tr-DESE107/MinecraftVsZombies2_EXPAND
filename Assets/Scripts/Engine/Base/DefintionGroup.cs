using System;
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
        public void Add<T>(T definition) where T : Definition
        {
            foreach (var list in lists)
            {
                if (list.CanAdd(definition))
                {
                    list.Add(definition);
                    return;
                }
            }
            var newList = new DefinitionList<T>();
            lists.Add(newList);
            newList.Add(definition);
        }
        public T GetDefinition<T>(NamespaceID id) where T : Definition
        {
            foreach (var list in lists)
            {
                if (list.CanGet<T>())
                {
                    return list.Get<T>(id);
                }
            }
            return default;
        }
        public T[] GetDefinitions<T>() where T : Definition
        {
            foreach (var list in lists)
            {
                if (list.CanGet<T>())
                {
                    return list.GetAll<T>();
                }
            }
            return Array.Empty<T>();
        }
        public Definition[] GetDefinitions()
        {
            return lists.SelectMany(l => l.GetAll()).ToArray();
        }
        protected List<DefinitionList> lists = new List<DefinitionList>();
    }
}
