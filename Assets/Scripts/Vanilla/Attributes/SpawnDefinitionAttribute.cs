using System;

namespace MVZ2.Vanilla.Level
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SpawnDefinitionAttribute : Attribute
    {
        public SpawnDefinitionAttribute(int spawnCost, int previewCount = 1)
        {
            SpawnCost = spawnCost;
            PreviewCount = previewCount;
        }
        public int SpawnCost { get; }
        public int PreviewCount { get; }
    }
}
