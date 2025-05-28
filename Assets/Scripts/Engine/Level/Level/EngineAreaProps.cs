using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    [PropertyRegistryRegion(PropertyRegions.level)]
    public static class EngineAreaProps
    {
        private static PropertyMeta<T> Get<T>(string name)
        {
            return new PropertyMeta<T>(name);
        }
        public static readonly PropertyMeta<float> GRID_WIDTH = Get<float>("GridWidth");
        public static readonly PropertyMeta<float> GRID_HEIGHT = Get<float>("GridHeight");
        public static readonly PropertyMeta<float> GRID_LEFT_X = Get<float>("GridLeftX");
        public static readonly PropertyMeta<float> GRID_BOTTOM_Z = Get<float>("GridBottomZ");
        public static readonly PropertyMeta<float> ENEMY_SPAWN_X = Get<float>("enemySpawnX");
        public static readonly PropertyMeta<float> ENTITY_LANE_Z_OFFSET = Get<float>("EntityLawnZOffset");
        public static readonly PropertyMeta<int> MAX_LANE_COUNT = Get<int>("MaxLaneCount");
        public static readonly PropertyMeta<int> MAX_COLUMN_COUNT = Get<int>("MaxColumnCount");
        public static readonly PropertyMeta<NamespaceID> CART_REFERENCE = Get<NamespaceID>("cartReference");
        public static readonly PropertyMeta<NamespaceID[]> AREA_TAGS = Get<NamespaceID[]>("areaTags");
        public static NamespaceID[] GetAreaTags(this AreaDefinition definition)
        {
            return definition.GetProperty<NamespaceID[]>(AREA_TAGS);
        }
        public static NamespaceID[] GetAreaTags(this LevelEngine level)
        {
            return level.GetProperty<NamespaceID[]>(AREA_TAGS);
        }
        public static NamespaceID GetCartReference(this LevelEngine level) => level.GetProperty<NamespaceID>(CART_REFERENCE);
    }
}
