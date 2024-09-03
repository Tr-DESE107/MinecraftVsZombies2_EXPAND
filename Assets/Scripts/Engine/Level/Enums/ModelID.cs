namespace PVZEngine.LevelManaging
{
    public static class ModelID
    {
        public const string TYPE_ENTITY = "entity";
        public const string TYPE_ARMOR = "armor";
        public static NamespaceID ToModelID(this NamespaceID id, string type)
        {
            return new NamespaceID(id.spacename, ConcatName(type, id.path));
        }
        public static string ConcatName(string type, string name)
        {
            return $"{type}.{name}";
        }
    }
}
