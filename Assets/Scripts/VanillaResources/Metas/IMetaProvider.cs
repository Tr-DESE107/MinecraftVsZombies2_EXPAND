using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Models;
using PVZEngine;

namespace MVZ2Logic.Games
{
    public interface IMetaProvider
    {
        public StageMeta GetStageMeta(NamespaceID id);
        public StageMeta[] GetModStageMetas(string spaceName);
        public EntityMeta GetEntityMeta(NamespaceID id);
        public EntityMeta[] GetModEntityMetas(string spaceName);
        public ModelMeta GetModelMeta(NamespaceID id);
        public ModelMeta[] GetModModelMetas(string spaceName);
    }
}
