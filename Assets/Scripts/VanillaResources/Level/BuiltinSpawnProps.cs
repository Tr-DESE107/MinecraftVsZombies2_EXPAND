using PVZEngine.Definitions;

namespace MVZ2.GameContent
{
    public static class BuiltinSpawnProps
    {
        public const string PREVIEW_COUNT = "previewCount";
        public static int GetPreviewCount(this SpawnDefinition def)
        {
            return def.TryGetProperty(PREVIEW_COUNT, out int value) ? value : 1;
        }
    }
}
