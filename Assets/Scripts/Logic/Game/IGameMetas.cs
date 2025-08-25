using MVZ2Logic.Commands;
using MVZ2Logic.Entities;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameMetas
    {
        public IShapeMeta GetShapeMeta(NamespaceID id);
        public IShapeMeta[] GetModShapeMetas(string spaceName);


        public string GetCommandNameByID(NamespaceID id);
        NamespaceID GetCommandIDByName(string name);
        public ICommandMeta GetCommandMeta(NamespaceID id);
        public NamespaceID[] GetAllCommandsID();

        public bool IsContraptionInAlmanac(NamespaceID id);
        public bool IsEnemyInAlmanac(NamespaceID id);
    }
}
