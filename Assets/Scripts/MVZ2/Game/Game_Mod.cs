using PVZEngine.Base;

namespace PVZEngine.Game
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
        private GameDefinitionGroup definitionGroup = new GameDefinitionGroup();
    }
}