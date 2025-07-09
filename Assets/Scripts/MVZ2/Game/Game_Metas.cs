using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using MVZ2Logic.SeedPacks;
using MVZ2Logic.Spawns;
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
        public IDifficultyMeta GetDifficultyMeta(NamespaceID stageID)
        {
            return metaProvider.GetDifficultyMeta(stageID);
        }
        public IAreaMeta GetAreaMeta(NamespaceID stageID)
        {
            return metaProvider.GetAreaMeta(stageID);
        }
        public IAreaMeta[] GetModAreaMetas(string spaceName)
        {
            return metaProvider.GetModAreaMetas(spaceName);
        }
        public IArmorSlotMeta GetArmorSlotMeta(NamespaceID stageID)
        {
            return metaProvider.GetArmorSlotMeta(stageID);
        }
        public IArmorMeta GetArmorMeta(NamespaceID stageID)
        {
            return metaProvider.GetArmorMeta(stageID);
        }
        public IArmorMeta[] GetModArmorMetas(string spaceName)
        {
            return metaProvider.GetModArmorMetas(spaceName);
        }
        public IEntityMeta GetEntityMeta(NamespaceID stageID)
        {
            return metaProvider.GetEntityMeta(stageID);
        }
        public IEntityMeta[] GetModEntityMetas(string spaceName)
        {
            return metaProvider.GetModEntityMetas(spaceName);
        }
        public IShapeMeta GetShapeMeta(NamespaceID stageID)
        {
            return metaProvider.GetShapeMeta(stageID);
        }
        public IShapeMeta[] GetModShapeMetas(string spaceName)
        {
            return metaProvider.GetModShapeMetas(spaceName);
        }
        public IEntityCounterMeta GetEntityCounterMeta(NamespaceID stageID)
        {
            return metaProvider.GetEntityCounterMeta(stageID);
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
        public ISeedOptionMeta GetSeedOptionMeta(NamespaceID stageID)
        {
            return metaProvider.GetSeedOptionMeta(stageID);
        }
        public ISeedOptionMeta[] GetModSeedOptionMetas(string spaceName)
        {
            return metaProvider.GetModSeedOptionMetas(spaceName);
        }
        public ISpawnMeta GetSpawnMeta(NamespaceID stageID)
        {
            return metaProvider.GetSpawnMeta(stageID);
        }
        public ISpawnMeta[] GetModSpawnMetas(string spaceName)
        {
            return metaProvider.GetModSpawnMetas(spaceName);
        }
        public IGridLayerMeta GetGridLayerMeta(NamespaceID id)
        {
            return metaProvider.GetGridLayerMeta(id);
        }
        public IGridErrorMeta GetGridErrorMeta(NamespaceID id)
        {
            return metaProvider.GetGridErrorMeta(id);
        }
        public IBlueprintErrorMeta GetBlueprintErrorMeta(NamespaceID id)
        {
            return metaProvider.GetBlueprintErrorMeta(id);
        }
        public bool IsContraptionInAlmanac(NamespaceID id)
        {
            return metaProvider.IsContraptionInAlmanac(id);
        }
        public bool IsEnemyInAlmanac(NamespaceID id)
        {
            return metaProvider.IsEnemyInAlmanac(id);
        }
        private IGameMetas metaProvider;
    }
}