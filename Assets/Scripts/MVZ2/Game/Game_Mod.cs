using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.Games
{
    public partial class Game
    {
        public T GetDefinition<T>(NamespaceID id) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(id);
        }
        public T[] GetDefinitions<T>() where T : Definition
        {
            return definitionGroup.GetDefinitions<T>();
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