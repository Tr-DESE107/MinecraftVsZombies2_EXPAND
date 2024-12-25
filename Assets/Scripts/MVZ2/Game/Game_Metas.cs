using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;

namespace MVZ2.Games
{
    public partial class Game
    {
        public IStageMeta GetStageMeta(NamespaceID stageID)
        {
            return metaProvider.GetStageMeta(stageID);
        }
        public IStageMeta[] GetModStageMetas(string spaceName)
        {
            return metaProvider.GetModStageMetas(spaceName);
        }
        public IAreaMeta GetAreaMeta(NamespaceID stageID)
        {
            return metaProvider.GetAreaMeta(stageID);
        }
        public IAreaMeta[] GetModAreaMetas(string spaceName)
        {
            return metaProvider.GetModAreaMetas(spaceName);
        }
        public IEntityMeta GetEntityMeta(NamespaceID stageID)
        {
            return metaProvider.GetEntityMeta(stageID);
        }
        public IEntityMeta[] GetModEntityMetas(string spaceName)
        {
            return metaProvider.GetModEntityMetas(spaceName);
        }
        public IArtifactMeta[] GetModArtifactMetas(string spaceName)
        {
            return metaProvider.GetModArtifactMetas(spaceName);
        }
        public IModelMeta GetModelMeta(NamespaceID stageID)
        {
            return metaProvider.GetModelMeta(stageID);
        }
        public IModelMeta[] GetModModelMetas(string spaceName)
        {
            return metaProvider.GetModModelMetas(spaceName);
        }
        private IGameMetas metaProvider;
    }
}