using MVZ2Logic.Commands;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
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
        public IShapeMeta GetShapeMeta(NamespaceID stageID)
        {
            return metaProvider.GetShapeMeta(stageID);
        }
        public IShapeMeta[] GetModShapeMetas(string spaceName)
        {
            return metaProvider.GetModShapeMetas(spaceName);
        }
        public ISeedOptionMeta GetSeedOptionMeta(NamespaceID stageID)
        {
            return metaProvider.GetSeedOptionMeta(stageID);
        }
        public ISeedOptionMeta[] GetModSeedOptionMetas(string spaceName)
        {
            return metaProvider.GetModSeedOptionMetas(spaceName);
        }
        public IEntitySeedMeta GetEntitySeedMeta(NamespaceID stageID)
        {
            return metaProvider.GetEntitySeedMeta(stageID);
        }
        public IEntitySeedMeta[] GetModEntitySeedMetas(string spaceName)
        {
            return metaProvider.GetModEntitySeedMetas(spaceName);
        }
        public ISpawnMeta GetSpawnMeta(NamespaceID stageID)
        {
            return metaProvider.GetSpawnMeta(stageID);
        }
        public ISpawnMeta[] GetModSpawnMetas(string spaceName)
        {
            return metaProvider.GetModSpawnMetas(spaceName);
        }
        }
        public string GetCommandNameByID(NamespaceID id)
        {
            return metaProvider.GetCommandNameByID(id);
        }
        public NamespaceID GetCommandIDByName(string name)
        {
            return metaProvider.GetCommandIDByName(name);
        }
        public ICommandMeta GetCommandMeta(NamespaceID id)
        {
            return metaProvider.GetCommandMeta(id);
        }
        public NamespaceID[] GetAllCommandsID()
        {
            return metaProvider.GetAllCommandsID();
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