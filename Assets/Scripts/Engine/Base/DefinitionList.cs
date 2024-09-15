using System.Collections.Generic;
using System.Linq;
using PVZEngine.Base;

namespace PVZEngine
{
    public abstract class DefinitionList
    {
        public abstract bool CanGet<T>() where T : Definition;
        public abstract T Get<T>(NamespaceID id) where T : Definition;
        public abstract T[] GetAll<T>() where T : Definition;
        public abstract Definition[] GetAll();
        public abstract bool CanAdd(Definition definition);
        public abstract void Add(Definition definition);
        public void AddRange(IEnumerable<Definition> definitions)
        {
            foreach (var def in definitions)
            {
                Add(def);
            }
        }
        public abstract void Clear();
        public abstract Definition GetDefinition(NamespaceID id);
        public abstract Definition[] GetDefinitions();
    }
    public class DefinitionList<T> : DefinitionList where T : Definition
    {
        public T GetDefinitionGeneric(NamespaceID id)
        {
            return definitions.FirstOrDefault(d => id == d.GetID());
        }
        public T[] GetDefinitionsGeneric()
        {
            return definitions.ToArray();
        }

        public override Definition[] GetDefinitions()
        {
            return GetDefinitionsGeneric();
        }
        public override Definition GetDefinition(NamespaceID id)
        {
            return GetDefinitionGeneric(id);
        }
        public override bool CanAdd(Definition definition)
        {
            return definition is T;
        }
        public override void Add(Definition definition)
        {
            if (definition is T tDef)
            {
                definitions.Add(tDef);
            }
        }
        public override bool CanGet<T1>()
        {
            return typeof(T).IsAssignableFrom(typeof(T1));
        }
        public override T1 Get<T1>(NamespaceID id)
        {
            return definitions.OfType<T1>().FirstOrDefault(d => d.GetID() == id);
        }
        public override T1[] GetAll<T1>()
        {
            return definitions.OfType<T1>().ToArray();
        }
        public override Definition[] GetAll()
        {
            return definitions.ToArray();
        }
        public override void Clear()
        {
            definitions.Clear();
        }
        private List<T> definitions = new List<T>();

    }
}
