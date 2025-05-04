using System.Collections.Generic;
using System.Linq;
using PVZEngine.Base;

namespace PVZEngine
{
    public class DefinitionList
    {
        public Definition[] GetAllDefinitions()
        {
            return definitions.ToArray();
        }
        public T[] GetAllDefinitions<T>() where T : Definition
        {
            return definitions.OfType<T>().ToArray();
        }
        public Definition GetDefinition(NamespaceID id)
        {
            foreach (var definition in definitions)
            {
                if (definition.GetID() == id)
                    return definition;
            }
            return null;
        }
        public T GetDefinition<T>(NamespaceID id) where T : Definition
        {
            foreach (var definition in definitions)
            {
                if (definition is T tDef && definition.GetID() == id)
                    return tDef;
            }
            return null;
        }
        public void Add(Definition definition)
        {
            definitions.Add(definition);
        }
        public void Clear()
        {
            definitions.Clear();
        }
        private List<Definition> definitions = new List<Definition>();

    }
}
