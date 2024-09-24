using MVZ2.Games;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.Modding
{
    public abstract class Mod : IContentProvider, IModLogic
    {
        public Mod(string nsp)
        {
            Namespace = nsp;
        }
        public virtual void PostGameInit() { }
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
        public string Namespace { get; }
        private GameDefinitionGroup definitionGroup = new GameDefinitionGroup();

    }
}
