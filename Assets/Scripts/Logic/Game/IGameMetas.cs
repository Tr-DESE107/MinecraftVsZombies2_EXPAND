using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IGameMetas
    {
        public IStageMeta GetStageMeta(NamespaceID id);
        public IStageMeta[] GetModStageMetas(string spaceName);
        public IEntityMeta GetEntityMeta(NamespaceID id);
        public IEntityMeta[] GetModEntityMetas(string spaceName);
        public IModelMeta GetModelMeta(NamespaceID id);
        public IModelMeta[] GetModModelMetas(string spaceName);
    }
}
