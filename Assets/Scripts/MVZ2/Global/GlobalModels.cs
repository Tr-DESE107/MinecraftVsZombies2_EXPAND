using MVZ2.Managers;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.GlobalGame
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
