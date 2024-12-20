namespace PVZEngine.Definitions
{
    public static class EngineAreaProps
    {
        public const string GRID_WIDTH = "GridWidth";
        public const string GRID_HEIGHT = "GridHeight";
        public const string GRID_LEFT_X = "GridLeftX";
        public const string GRID_BOTTOM_Z = "GridBottomZ";
        public const string MAX_LANE_COUNT = "MaxLaneCount";
        public const string MAX_COLUMN_COUNT = "MaxColumnCount";
        public const string CART_REFERENCE = "cartReference";
        public const string ENEMY_SPAWN_X = "enemySpawnX";
        public const string AREA_TAGS = "areaTags";
        public static NamespaceID[] GetAreaTags(this AreaDefinition definition)
        {
            return definition.GetProperty<NamespaceID[]>(AREA_TAGS);
        }
    }
}
