using MVZ2.Managers;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.GlobalGames
{
    public class GlobalModels : IGlobalModels
    {
        public GlobalModels(MainManager main)
        {
            this.main = main;
        }
        public bool ModelExists(NamespaceID id)
        {
            return main.ResourceManager.GetModelMeta(id) != null;
        }
        private MainManager main;

    }
}
