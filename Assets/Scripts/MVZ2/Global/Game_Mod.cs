using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.GlobalGames
{
    public partial class GlobalGame
    {
        public T GetDefinition<T>(string type, NamespaceID id) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(type, id);
        }
        public T[] GetDefinitions<T>(string type) where T : Definition
        {
            return definitionGroup.GetDefinitions<T>(type);
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        public void AddMod(IModLogic mod)
        {
            foreach (var def in mod.GetDefinitions())
            {
                definitionGroup.Add(def);
            }
        }
        private DefinitionGroup definitionGroup = new DefinitionGroup();
    }
}