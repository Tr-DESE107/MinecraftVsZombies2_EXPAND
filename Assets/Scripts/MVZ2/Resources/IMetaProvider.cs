using PVZEngine;

namespace MVZ2.Resources
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
