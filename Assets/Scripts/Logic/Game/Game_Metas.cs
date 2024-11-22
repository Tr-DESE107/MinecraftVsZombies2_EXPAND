using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;

namespace MVZ2Logic.Games
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
        public ModelMeta GetModelMeta(NamespaceID stageID)
        {
            return metaProvider.GetModelMeta(stageID);
        }
        public ModelMeta[] GetModModelMetas(string spaceName)
        {
            return metaProvider.GetModModelMetas(spaceName);
        }
        private IMetaProvider metaProvider;
    }
}