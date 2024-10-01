using MVZ2.Modding;
using MVZ2.Resources;
using PVZEngine;
using PVZEngine.Base;

namespace MVZ2.Games
{
    public partial class Game
    {
        public StageMeta GetStageMeta(NamespaceID stageID)
        {
            return metaProvider.GetStageMeta(stageID);
        }
        public StageMeta[] GetModStageMetas(string spaceName)
        {
            return metaProvider.GetModStageMetas(spaceName);
        }
        public EntityMeta GetEntityMeta(NamespaceID stageID)
        {
            return metaProvider.GetEntityMeta(stageID);
        }
        public EntityMeta[] GetModEntityMetas(string spaceName)
        {
            return metaProvider.GetModEntityMetas(spaceName);
        }
        private IMetaProvider metaProvider;
    }
}