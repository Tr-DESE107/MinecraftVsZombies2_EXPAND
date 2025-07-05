using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2Logic.Entities
{
    [PropertyRegistryRegion(PropertyRegions.entity)]
    public static class LogicEntityProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        #region 形状ID
        public static readonly PropertyMeta<NamespaceID> SHAPE = Get<NamespaceID>("shape");
        public static NamespaceID GetShapeID(this EntityDefinition definition)
        {
            return definition.GetProperty<NamespaceID>(SHAPE);
        }
        public static NamespaceID GetShapeID(this Entity entity, bool ignoreBuffs = false)
        {
            return entity.GetProperty<NamespaceID>(SHAPE, ignoreBuffs: ignoreBuffs);
        }
        public static void SetShapeID(this Entity entity, NamespaceID value)
        {
            entity.SetProperty(SHAPE, value);
        }
        #endregion

    }
}
