using PVZEngine;

namespace MVZ2.Resources
{
    public interface IMetaProvider
    {
        public StageMeta GetStageMeta(NamespaceID stageID);
        public StageMeta[] GetModStageMetas(string spaceName);
        public EntityMeta GetEntityMeta(NamespaceID stageID);
        public EntityMeta[] GetModEntityMetas(string spaceName);
    }
}
