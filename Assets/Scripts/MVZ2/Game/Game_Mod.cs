using System.Collections.Generic;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Grids;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.Games
{
    public partial class Game
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
        public int GetGridLayerPriority(NamespaceID layer)
        {
            var meta = GetGridLayerMeta(layer);
            if (meta == null)
                return 0;
            return meta.Priority;
        }
        public string GetGridErrorMessage(NamespaceID error)
        {
            var meta = GetGridErrorMeta(error);
            if (meta == null)
                return null;
            return meta.Message;
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