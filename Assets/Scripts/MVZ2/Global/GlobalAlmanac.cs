using MVZ2.Managers;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.GlobalGame
{
    public class GlobalAlmanac : IGlobalAlmanac
    {
        public GlobalAlmanac(MainManager main)
        {
            this.main = main;
        }
        public bool IsContraptionInAlmanac(NamespaceID id)
        {
            return main.ResourceManager.IsContraptionInAlmanac(id);
        }

        public bool IsEnemyInAlmanac(NamespaceID id)
        {
            return main.ResourceManager.IsEnemyInAlmanac(id);
        }
        private MainManager main;

    }
}
