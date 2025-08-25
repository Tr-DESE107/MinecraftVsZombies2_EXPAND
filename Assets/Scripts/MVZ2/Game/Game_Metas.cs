using MVZ2Logic.Commands;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Games
{
    public partial class Game
    {
        public IShapeMeta GetShapeMeta(NamespaceID stageID)
        {
            return metaProvider.GetShapeMeta(stageID);
        }
        public IShapeMeta[] GetModShapeMetas(string spaceName)
        {
            return metaProvider.GetModShapeMetas(spaceName);
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