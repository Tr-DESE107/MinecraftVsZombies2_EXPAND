using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using MVZ2Logic.SeedPacks;
using MVZ2Logic.Spawns;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameMetas
    {
        public IStageMeta GetStageMeta(NamespaceID id);
        public IStageMeta[] GetModStageMetas(string spaceName);
        public IDifficultyMeta GetDifficultyMeta(NamespaceID id);
        public IAreaMeta GetAreaMeta(NamespaceID id);
        public IAreaMeta[] GetModAreaMetas(string spaceName);
        public IEntityMeta GetEntityMeta(NamespaceID id);
        public IEntityMeta[] GetModEntityMetas(string spaceName);
        public IArtifactMeta[] GetModArtifactMetas(string spaceName);
        public IModelMeta GetModelMeta(NamespaceID id);
        public IModelMeta[] GetModModelMetas(string spaceName);
        public ISeedOptionMeta GetSeedOptionMeta(NamespaceID id);
        public ISeedOptionMeta[] GetModSeedOptionMetas(string spaceName);
        public ISpawnMeta GetSpawnMeta(NamespaceID id);
        public ISpawnMeta[] GetModSpawnMetas(string spaceName);
        public IGridLayerMeta GetGridLayerMeta(NamespaceID id);
        public IGridErrorMeta GetGridErrorMeta(NamespaceID id);
        public bool IsContraptionInAlmanac(NamespaceID id);
        public bool IsEnemyInAlmanac(NamespaceID id);
    }
}
