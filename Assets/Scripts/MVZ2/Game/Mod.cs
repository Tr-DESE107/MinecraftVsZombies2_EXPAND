using MVZ2.Save;
using PVZEngine.Base;

namespace PVZEngine.Game
{
    public abstract class Mod : IContentProvider, IModLogic
    {
        public Mod(Game game, string nsp)
        {
            Game = game;
            Namespace = nsp;
        }
        public virtual void Init(Game game) { }
        public abstract ModSaveData CreateSaveData();
        public abstract ModSaveData LoadSaveData(string json);
        protected void AddDefinition(Definition def)
        {
            definitionGroup.Add(def);
        }
        public T GetDefinition<T>(NamespaceID defRef) where T : Definition
        {
            return definitionGroup.GetDefinition<T>(defRef);
        }
        public T[] GetDefinitions<T>() where T : Definition
        {
            return definitionGroup.GetDefinitions<T>();
        }
        public Definition[] GetDefinitions()
        {
            return definitionGroup.GetDefinitions();
        }
        public Game Game { get; }
        public string Namespace { get; }
        private GameDefinitionGroup definitionGroup = new GameDefinitionGroup();

    }
}
